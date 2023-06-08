using UnityEngine;
using System.Collections;
public class SplashAnimation : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod]
    static void LoadAnimation()
    {
#if UNITY_ANDROID || UNITY_IOS
        Handheld.PlayFullScreenMovie("LudoSplash.mp4", new Color(0 / 255, 32 / 255, 105 / 255), FullScreenMovieControlMode.Hidden, FullScreenMovieScalingMode.AspectFit);
#endif
    }
}