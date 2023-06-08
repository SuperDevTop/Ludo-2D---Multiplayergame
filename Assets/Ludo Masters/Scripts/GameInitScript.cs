using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameInitScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {


        if (GameManager.Instance.roomOwner)
        {
            PhotonNetwork.RaiseEvent(198, null, new Photon.Realtime.RaiseEventOptions(), new ExitGames.Client.Photon.SendOptions() { Reliability = true });
        }
        else
        {
            // for (int i = 0; i < GameManager.Instance.initPositions.Length; i++) {
            //     GameManager.Instance.balls[i + 1].GetComponent<Rigidbody>().transform.position = GameManager.Instance.initPositions[i];
            //     GameManager.Instance.balls[i + 1].SetActive(true);
            // }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
