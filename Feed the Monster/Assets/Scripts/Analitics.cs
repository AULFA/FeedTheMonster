﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Messaging;
using Firebase.Analytics;

public class Analitics : MonoBehaviour
{
	public static Analitics Instance;
	private delegate void DeferredEvent();
	private DeferredEvent deferred;
	public UINotificationPopup popup;
	public bool isReady = false;
	private const int MAXUSERPROPERTIES = 25;
	AndroidJavaObject activity;

	void Awake()
    {
        Instance = this;

		// we need to explicitly exclude the editor to prevent Player crashes
	}

	// Use this for initialization
	void Start ()
	{

		#if UNITY_ANDROID && !UNITY_EDITOR
		/*
		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
		{
			var dependencyStatus = task.Result;
			if (dependencyStatus == Firebase.DependencyStatus.Available)
			{
				var app = FirebaseApp.DefaultInstance;
				Instance.isReady = true;
				deferred.Invoke(); // log events that were captured before initialization
				Debug.Log("successfully initialized firebase");
			}
			else
			{
				Debug.LogError(System.String.Format(
					"Could not resolve all Firebase dependencies: {0}", dependencyStatus));
			}
		});
		*/
		AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
		#endif

	}

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {

        }
    }

    void OnDisable() {

	}


	public void treckScreen (string screenName)
	{
		#if  UNITY_ANDROID && !UNITY_EDITOR
			FirebaseAnalytics.SetCurrentScreen (screenName, null);
		#endif

		#if  UNITY_ANDROID
			activity.Call("tagScreen", new object[1] {screenName});
		#endif

	}


	public void treckEvent (AnaliticsCategory category, AnaliticsAction action, string label, long value = 0)
	{
		treckEvent (category, action.ToString(), label, value);
	}

	public void treckEvent (AnaliticsCategory category, string action, string label, long value = 0)
	{
		#if UNITY_ANDROID
			activity.Call("logEvent", new object[4] {category.ToString(), action, label, value.ToString()});
		#endif
	}

	public void logNotification(FirebaseMessage message)
    {
		if(!isReady)
        {
			deferred += () =>
			{
				logNotification(message);
			};
			return;
        }
		FirebaseAnalytics.LogEvent("notification_received", new Parameter[]
		{
			new Parameter(
				"message_id", message.MessageId
				),
			new Parameter(
				"message_type", message.MessageType
				),
			new Parameter (
				"notif_opened", message.NotificationOpened.ToString()
				),
		});
    }

	public void setUserProperty(string prop, string val)
    {
		FirebaseAnalytics.SetUserProperty(prop, val);
    }
}
