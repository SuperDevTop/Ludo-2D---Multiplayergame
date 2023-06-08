using UnityEngine;
using Facebook.Unity;
using System.Collections.Generic;

public class FacebookController : MonoBehaviour
{
    static FacebookController facebook = null;

    void Awake()
    {
        if (facebook)
        {
            Destroy(gameObject);
            return;
        }

        facebook = this;
        DontDestroyOnLoad(gameObject);

        if (FB.IsInitialized)
        {
            FB.ActivateApp();
            LogApplicationStart();
        }
        else
        {
            FB.Init(() =>
            {
                FB.ActivateApp();
                LogApplicationStart();
            });
        }

        FB.Mobile.SetAutoLogAppEventsEnabled(true);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
                LogApplicationResume();
            }
            else
            {
                FB.Init(() =>
                {
                    FB.ActivateApp();
                    LogApplicationResume();
                });
            }
        }
    }

    private void LogApplicationStart()
    {
        FB.LogAppEvent("Application Start", null, new Dictionary<string, object>()
        {
            { "Platform", Application.platform }
        });
    }

    private void LogApplicationResume()
    {
        FB.LogAppEvent("Application Resume", null, new Dictionary<string, object>()
        {
            { "Platform", Application.platform }
        });
    }
}