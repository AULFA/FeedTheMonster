using UnityEngine;
using UnityEngine.Android;
using System.Collections;
using System.Collections.Generic;

public class Analitics : MonoBehaviour
{
	public static Analitics Instance;
	private delegate void DeferredEvent();
	private DeferredEvent deferred;
	public UINotificationPopup popup;
	public bool isReady = false;
	private const int MAXUSERPROPERTIES = 25;
	AndroidJavaObject logger;

	void Awake()
    {
        Instance = this;

		// we need to explicitly exclude the editor to prevent Player crashes
	}

	// Use this for initialization
	void Start ()
	{

		#if UNITY_ANDROID && !UNITY_EDITOR
		if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
		{
			Permission.RequestUserPermission(Permission.ExternalStorageRead);
		}
		if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
		{
			Permission.RequestUserPermission(Permission.ExternalStorageWrite);
		}
		#endif
		
		init();
	}

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {

        }
    }

    void OnDisable() {

	}
	
	void init() {
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
		AndroidJavaClass loggerClass = new AndroidJavaClass("au.org.libraryforall.logger.Logger");
		logger = loggerClass.CallStatic<AndroidJavaObject>("getInstance", new object[2] {"FeedTheMonster", context});
		#endif
	}


	public void treckScreen (string screenName)
	{
		if (logger == null) {
			init();
		}
		#if  UNITY_ANDROID && !UNITY_EDITOR
			logger.Call("tagScreen", new object[1] {screenName});
		#endif

	}


	public void treckEvent (AnaliticsCategory category, AnaliticsAction action, string label, long value = 0)
	{
		treckEvent (category, action.ToString(), label, value);
	}

	public void treckEvent (AnaliticsCategory category, string action, string label, long value = 0)
	{
		if (logger == null) {
			init();
		}
		#if UNITY_ANDROID && !UNITY_EDITOR
			logger.Call("logEvent", new object[4] {category.ToString(), action, label, (double) value});
		#endif
	}
}
