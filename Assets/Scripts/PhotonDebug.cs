using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PhotonDebug : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod]
    static void OnLoad()
    {
        //DontDestroyOnLoad(Instantiate(Resources.Load("PhotonDebug") as GameObject));
    }

    public Text text;

    void Update()
    {
        text.text = GetStatus();
    }

    string GetStatus()
    {
        return "Photon Debug\n" +
            (PhotonNetwork.IsConnected ? "Connected\n" : "Not Connected\n") +
            (PhotonNetwork.InRoom ? "Room: " + PhotonNetwork.CurrentRoom.Name + "\n" : "") +
            (PhotonNetwork.InRoom ? "Master Client: " + PhotonNetwork.IsMasterClient + "\n" : "") +
            (PhotonNetwork.InRoom ? "Players: " + PhotonNetwork.CurrentRoom.PlayerCount + "\n" : "") +
            (PhotonNetwork.IsConnected ? "Ping: " + PhotonNetwork.GetPing() : "");
    }
}
