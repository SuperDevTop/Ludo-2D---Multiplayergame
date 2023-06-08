using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Photon.Pun;

public class CheckIfPlayerIsFriend : MonoBehaviour {

    public GameObject AddFriendButton;
    public GameObject mainObject;
    // Use this for initialization
    void Start() {
        GameManager.Instance.smallMenu = mainObject;
        GameManager.Instance.friendButtonMenu = AddFriendButton;

        if (!GameManager.Instance.offlineMode) {
            GetFriendsListRequest request = new GetFriendsListRequest();
            request.IncludeFacebookFriends = true;
            PlayFabClientAPI.GetFriendsList(request, (result) => {
                var friends = result.Friends;
                foreach (var friend in friends) {
                    if (PhotonNetwork.PlayerListOthers[0].NickName.Equals(friend.FriendPlayFabId)) {
                        Debug.Log("Already friends");
                        AddFriendButton.SetActive(false);
                        mainObject.GetComponent<RectTransform>().sizeDelta = new Vector2(mainObject.GetComponent<RectTransform>().sizeDelta.x, 260.0f);
                        break;
                    }
                }
            }, OnPlayFabError);
        } else {
            AddFriendButton.SetActive(false);
            mainObject.GetComponent<RectTransform>().sizeDelta = new Vector2(mainObject.GetComponent<RectTransform>().sizeDelta.x, 260.0f);
        }

    }

    void OnPlayFabError(PlayFabError error) {
        Debug.Log("Playfab Error: " + error.ErrorMessage);
    }

    // Update is called once per frame
    void Update() {

    }
}
