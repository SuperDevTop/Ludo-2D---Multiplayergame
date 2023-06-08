using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TestMatching : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnConnectedToMaster()
    {
        print("OnConnectedToMaster");

        PhotonNetwork.CreateRoom(null);
    }

    public override void OnCreatedRoom()
    {
        print("Room created: " + PhotonNetwork.CurrentRoom.Name);
    }
}
