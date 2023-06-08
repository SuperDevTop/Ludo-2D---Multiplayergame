using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebViewContainer : MonoBehaviour
{
    public WebViewObject webView;

    public float left;
    public float top;
    public float right;
    public float bottom;

    public void Begin(string url)
    {
#if UNITY_ANDROID
        webView.Init(err: (str) => print("Error:- "+ str),started: (msg) =>
            {
                Debug.Log(string.Format("CallOnStarted[{0}]", msg));
            },
            hooked: (msg) =>
            {
                Debug.Log(string.Format("CallOnHooked[{0}]", msg));
            },
            ld: (msg) =>
            {
                Debug.Log("Message:- "+msg);
            });
#else
        webView.Init(err: (str) => print("Error:- "+ str), enableWKWebView: true);
#endif

        webView.SetMargins((int)(Screen.width * left),
            (int)(Screen.height * top),
            (int)(Screen.width * right),
            (int)(Screen.height * bottom));

        webView.SetVisibility(true);
        Common.ScreenLog.ForceLog("Begin method"+url);
        
        Debug.Log("Begin method:- "+ url);

        webView.LoadURL(url);
    }

    public void OnBackButton()
    {
        if (webView.CanGoBack())
        {
            webView.GoBack();
        }
        else
        {
            Destroy(webView);
            Destroy(gameObject);
        }
    }

    public void OnCloseButton()
    {
        Destroy(webView);
        Destroy(gameObject);
    }
}