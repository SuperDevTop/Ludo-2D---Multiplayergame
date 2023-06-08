using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkOpener : MonoBehaviour
{

    //public string Link;


    public void OpenLink(string Link)
    {
        //Application.OpenURL(Link);
        Debug.Log("Open link method");
        WebViewContainer webView = Instantiate(Resources.Load<WebViewContainer>("WebView"));

        webView.Begin(Link);

        //InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
        //options.displayURLAsPageTitle = false;
        //options.pageTitle = "";

        //InAppBrowser.OpenURL(Link, options);

        //WebViewHandler.Instance.openURL(Link);
        //Application.OpenURL(Link);
    }

    public static void OpenLinkStatic(string Link)
    {
        //Application.OpenURL(Link);

        WebViewContainer webView = Instantiate(Resources.Load<WebViewContainer>("WebView"));
        Debug.Log("Open link  static method");
        webView.Begin(Link);

        //InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
        //options.displayURLAsPageTitle = false;
        //options.pageTitle = "";

        //InAppBrowser.OpenURL(Link, options);

        //WebViewHandler.Instance.openURL(Link);
        //Application.OpenURL(Link);
    }


}
