using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections;

public class PingView : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod]
    static void Load()
    {
        DontDestroyOnLoad(Instantiate(Resources.Load<GameObject>("UI/PingView")));
    }

    public Text text;

    public float interval;
    public string formatExcellent;
    public string formatAverage;
    public string formatBad;

    IEnumerator Start()
    {
        while (true)
        {
            if (PhotonNetwork.InRoom)
            {
                int ping = PhotonNetwork.GetPing();
                if (ping < 200) text.text = string.Format(formatExcellent, ping);
                else if (ping < 500) text.text = string.Format(formatAverage, ping);
                else text.text = string.Format(formatBad, ping);
            }
            else
            {
                text.text = string.Empty;
            }
            yield return new WaitForSeconds(interval);
        }
    }
}
