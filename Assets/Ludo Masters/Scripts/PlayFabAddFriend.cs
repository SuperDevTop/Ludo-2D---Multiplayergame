using UnityEngine;
using System.Collections;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine.SceneManagement;
using AssemblyCSharp;
using Photon.Pun;

public class PlayFabAddFriend : MonoBehaviour
{

    public GameObject menuObject;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }

    public void AddFriend()
    {
        menuObject.GetComponent<Animator>().Play("hideMenuAnimation");
        if (!GameManager.Instance.offlineMode)
        {
            PhotonNetwork.RaiseEvent(192, 1, new Photon.Realtime.RaiseEventOptions(), new ExitGames.Client.Photon.SendOptions() { Reliability = true });



            AddFriendRequest request = new AddFriendRequest()
            {
                FriendPlayFabId = PhotonNetwork.PlayerListOthers[0].NickName
            };



            PlayFabClientAPI.AddFriend(request, (result) =>
            {
                Debug.Log("Added friend successfully");
                GameManager.Instance.friendButtonMenu.SetActive(false);
                GameManager.Instance.smallMenu.GetComponent<RectTransform>().sizeDelta = new Vector2(GameManager.Instance.smallMenu.GetComponent<RectTransform>().sizeDelta.x, 260.0f);
            }, (error) =>
            {
                Debug.Log("Error adding friend: " + error.Error);
            }, null);
        }

    }

    public void showMenu()
    {
        menuObject.GetComponent<Animator>().Play("ShowMenuAnimation");
    }

    public void hideMenu()
    {
        menuObject.GetComponent<Animator>().Play("hideMenuAnimation");
    }

    public void LeaveGame()
    {
        // if (StaticStrings.showAdWhenLeaveGame)
        //     AdsManager.Instance.adsScript.ShowAd();
        SceneManager.LoadScene("MenuScene");
        //Photon.Pun.PhotonNetwork.BackgroundTimeout = StaticStrings.photonDisconnectTimeoutLong; ;
        Debug.Log("Timeout 3");
        //GameManager.Instance.cueController.removeOnEventCall();
        Photon.Pun.PhotonNetwork.LeaveRoom();

        GameManager.Instance.playfabManager.roomOwner = false;
        GameManager.Instance.roomOwner = false;
        GameManager.Instance.resetAllData();

    }
}
