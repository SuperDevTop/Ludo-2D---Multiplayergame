using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TestingModeView : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod]
    static void Load()
    {
        DontDestroyOnLoad(Instantiate(Resources.Load<GameObject>("UI/TestingModeView")));
    }

    public Text text;

    public float interval;

    IEnumerator Start()
    {
        while (true)
        {
            text.text = TestingMode.freeMatches ? "Free Matches" : string.Empty;
            yield return new WaitForSeconds(interval);
        }
    }
}
