using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

//using ExitGames.Client.Photon.Chat;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
using PlayFab;
using System.Collections.Generic;
#if UNITY_ANDROID || UNITY_IOS
using UnityEngine.Advertisements;
#endif
using AssemblyCSharp;

public class InitMenuScript : MonoBehaviour
{
    public GameObject rateWindow;
    public GameObject FacebookLinkReward;
    public GameObject rewardDialogText;
    public GameObject FacebookLinkButton;
    public GameObject playerName;
    public GameObject videoRewardText;
    public GameObject playerAvatar;
    public GameObject fbFriendsMenu;
    public GameObject matchPlayer;
    public GameObject backButtonMatchPlayers;
    public GameObject MatchPlayersCanvas;
    public GameObject menuCanvas;
    public GameObject tablesCanvas;
    public GameObject gameTitle;
    public GameObject changeDialog;
    public GameObject inputNewName;
    public GameObject tooShortText;
    public GameObject coinsText;
    public GameObject coinsTextShop;
    public GameObject coinsTab;
    public GameObject TheMillButton;
    public GameObject dialog;
    // Use this for initialization
    public GameObject GameConfigurationScreen;
    public GameObject FourPlayerMenuButton;
    public GameObject popupScreen;
    public GameObject updateDataPopup;
    public Text text;
    int index;
    private StaticGameVariablesController staticController;
    void Start()
    {
        Debug.Log("Init menu script");
        Common.ScreenLog.ForceLog("Menu Screen");
        if (PlayerPrefs.GetInt(StaticStrings.SoundsKey, 0) == 0)
        {
            AudioListener.volume = 1;
        }
        else
        {
            AudioListener.volume = 0;
        }


        //FacebookLinkReward.GetComponent<Text>().text = "+ " + StaticStrings.CoinsForLinkToFacebook;

        if (!StaticStrings.isFourPlayerModeEnabled)
        {
            FourPlayerMenuButton.SetActive(false);
        }

        GameManager.Instance.FacebookLinkButton = FacebookLinkButton;

        GameManager.Instance.dialog = dialog;
        videoRewardText.GetComponent<Text>().text = "+" + StaticStrings.rewardForVideoAd;
        GameManager.Instance.tablesCanvas = tablesCanvas;
        GameManager.Instance.facebookFriendsMenu = fbFriendsMenu.GetComponent<FacebookFriendsMenu>(); ;
        GameManager.Instance.matchPlayerObject = matchPlayer;
        GameManager.Instance.backButtonMatchPlayers = backButtonMatchPlayers;
        playerName.GetComponent<Text>().text = GameManager.Instance.nameMy;
        GameManager.Instance.MatchPlayersCanvas = MatchPlayersCanvas;

        //staticController = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>();
        //GameManager.Instance.avatarMy = staticController.avatars[0];

        if (PlayerPrefs.GetString("LoggedType").Equals("Facebook"))
        {
            FacebookLinkButton.SetActive(false);
        }

        if (GameManager.Instance.avatarMy != null)
        {
            playerAvatar.GetComponent<Image>().sprite = GameManager.Instance.avatarMy;
        }

        GameManager.Instance.myAvatarGameObject = playerAvatar;
        GameManager.Instance.myNameGameObject = playerName;

        GameManager.Instance.coinsTextMenu = coinsText;

        GameManager.Instance.coinsTextShop = coinsTextShop;
        GameManager.Instance.initMenuScript = this;


        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("LoggedType", "email");
        data.Add(MyPlayerData.AvatarIndexKey, GameManager.Instance.myPlayerData.GetAvatarIndex());
        data.Add(MyPlayerData.PlayerName, GameManager.Instance.nameMy);

        GameManager.Instance.myPlayerData.UpdateUserData(data, updateDataPopup, this);


        if (StaticStrings.hideCoinsTabInShop)
        {
            coinsTab.SetActive(false);
        }

#if UNITY_WEBGL
        coinsTab.SetActive(false);
#endif

        rewardDialogText.GetComponent<Text>().text = "1 Video = " + StaticStrings.rewardForVideoAd + " Coins";
        //coinsText.GetComponent<Text>().text = GameManager.Instance.myPlayerData.GetCoins() + "";



        Debug.Log("Load ad menu");
        //if (AdsManager.Instance.adsScript)
        //    AdsManager.Instance.adsScript.ShowAd(AdLocation.GameStart);

        if (PlayerPrefs.GetInt("GamesPlayed", 1) % 8 == 0 && PlayerPrefs.GetInt("GameRated", 0) == 0)
        {
            rateWindow.SetActive(true);
            PlayerPrefs.SetInt("GamesPlayed", PlayerPrefs.GetInt("GamesPlayed", 1) + 1);
        }

    }

    public void QuitApp()
    {
        PlayerPrefs.SetInt("GameRated", 1);
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=" + StaticStrings.AndroidPackageName);
#elif UNITY_IPHONE
        Application.OpenURL("itms-apps://itunes.apple.com/app/id" + StaticStrings.ITunesAppID);
#endif
        //Application.Quit();
    }


    public void LinkToFacebook()
    {
        GameManager.Instance.facebookManager.FBLinkAccount();
    }
    public void SetInformation(int index)
    {
        popupScreen.SetActive(true);
        Common.ScreenLog.ForceLog(index + "player game selected");

        switch (index)
        {
            case 0:
                SetGameInformation("You are entering the 2 players game room. £1.10 will be removed from your account as soon as an opponent is found. If you win, you'll receive £2.");
                break;
            case 1:
                SetGameInformation("You are entering the 4 players game room. £1.10 will be removed from your account as soon as an opponent is found. If you win, you'll receive £4.");
                break;
            case 2:
                SetGameInformation("Do you want to start game in private room?");
                break;
        }
        this.index = index;
    }

    public void OnRestoreButton()
    {
        GameManager.Instance.IAPControl.RestorePurchases();
    }

    public void OnYesButton()
    {
        if (TestingMode.freeMatches)
        {
            popupScreen.SetActive(false);
            ShowGameConfiguration(index);
            return;
        }

        if (GameManager.Instance.type == MyGameType.TwoPlayer)
        {
            if (Ludo.IN.GameManager.Instance.Genie_user_funds >= 1.10f)
            {
                popupScreen.SetActive(false);
                ShowGameConfiguration(index);
            }
            else
            {
                popupScreen.SetActive(false);
                dialog.SetActive(true);
            }
        }
        else if (GameManager.Instance.type == MyGameType.FourPlayer)
        {
            if (Ludo.IN.GameManager.Instance.Genie_user_funds >= 1.10f)
            {
                popupScreen.SetActive(false);
                ShowGameConfiguration(index);
            }
            else
            {
                popupScreen.SetActive(false);
                dialog.SetActive(true);
            }
        }
    }

    public void OnNoButton()
    {
        popupScreen.SetActive(false);
    }

    public void SetGameInformation(string information)
    {
        text.text = information;
    }

    public void ShowGameConfiguration(int index)
    {
        switch (index)
        {
            case 0:
                GameManager.Instance.type = MyGameType.TwoPlayer;
                break;
            case 1:
                GameManager.Instance.type = MyGameType.FourPlayer;
                break;
            case 2:
                GameManager.Instance.type = MyGameType.Private;
                break;
        }
        GameConfigurationScreen.SetActive(true);
        //AdsManager.Instance.adsScript.ShowAd(AdLocation.GamePropertiesWindow);
    }

    public void TakeScreenshot()
    {
        ScreenCapture.CaptureScreenshot("TestScreenshot.png");
    }


    //// Update is called once per frame
    //void Update()
    //{
    //    coinsText.GetComponent<Text>().text = Ludo.IN.GameManager.Instance.Genie_user_funds.ToString("F2");
    //}

    public void showAdStore()
    {
        //AdsManager.Instance.adsScript.ShowAd(AdLocation.StoreWindow);
    }

    public void backToMenuFromTableSelect()
    {
        GameManager.Instance.offlineMode = false;
        tablesCanvas.SetActive(false);
        menuCanvas.SetActive(true);
        gameTitle.SetActive(true);
    }

    public void showSelectTableScene(bool challengeFriend)
    {
        if (!challengeFriend)
            GameManager.Instance.inviteFriendActivated = false;

        //AdsManager.Instance.adsScript.ShowAd(AdLocation.GameStart);
        if (GameManager.Instance.offlineMode)
        {
            TheMillButton.SetActive(false);
        }
        else
        {
            TheMillButton.SetActive(true);
        }
        menuCanvas.SetActive(false);
        tablesCanvas.SetActive(true);
        gameTitle.SetActive(false);
    }

    public void playOffline()
    {
        //GameManager.Instance.tableNumber = 0;
        GameManager.Instance.offlineMode = true;
        GameManager.Instance.roomOwner = true;
        showSelectTableScene(false);
        //SceneManager.LoadScene(GameManager.Instance.GameScene);
    }

    public void switchUser()
    {
        GameManager.Instance.playfabManager.destroy();
        GameManager.Instance.facebookManager.destroy();
        GameManager.Instance.connectionLost.destroy();

        GameManager.Instance.avatarMy = null;
        Photon.Pun.PhotonNetwork.Disconnect();

        PlayerPrefs.DeleteAll();
        GameManager.Instance.resetAllData();
        LocalNotification.ClearNotifications();
        //GameManager.Instance.myPlayerData.GetCoins() = 0;
        SceneManager.LoadScene("LoginSplash");
    }

    public void showChangeDialog()
    {
        changeDialog.SetActive(true);
    }

    public void changeUserName()
    {
        Debug.Log("Change Nickname");

        string newName = inputNewName.GetComponent<Text>().text;
        if (newName.Equals(StaticStrings.addCoinsHackString))
        {
            GameManager.Instance.playfabManager.addCoinsRequest(1000000);
            changeDialog.SetActive(false);
        }
        else
        {
            if (newName.Length > 0)
            {
                UpdateUserTitleDisplayNameRequest displayNameRequest = new UpdateUserTitleDisplayNameRequest()
                {
                    //DisplayName = newName
                    DisplayName = GameManager.Instance.playfabManager.PlayFabId
                };

                PlayFabClientAPI.UpdateUserTitleDisplayName(displayNameRequest, (response) =>
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("PlayerName", newName);
                    UpdateUserDataRequest userDataRequest = new UpdateUserDataRequest()
                    {
                        Data = data,
                        Permission = UserDataPermission.Public
                    };

                    PlayFabClientAPI.UpdateUserData(userDataRequest, (result1) =>
                    {
                        Common.ScreenLog.ForceLog("Data updated successfull 0: " + result1.CustomData);
                        Debug.Log("Title Display name updated successfully");
                        PlayerPrefs.SetString("GuestPlayerName", newName);
                        PlayerPrefs.Save();
                        GameManager.Instance.nameMy = newName;
                        playerName.GetComponent<Text>().text = newName;

                    }, (error1) =>
                    {
                        Common.ScreenLog.ForceLog("Data update error 0: " + error1.ErrorMessage);
                    }, null);

                }, (error) =>
                {
                    Common.ScreenLog.ForceLog("Data update error 1");
                    Debug.Log("Title Display name updated error: " + error.ErrorMessage);

                }, null);

                changeDialog.SetActive(false);
            }
            else
            {
                tooShortText.SetActive(true);
            }
        }



    }

    public void startQuickGame()
    {
        GameManager.Instance.facebookManager.startRandomGame();
    }

    public void startQuickGameTableNumer(int tableNumer, int fee)
    {
        GameManager.Instance.payoutCoins = fee;
        GameManager.Instance.tableNumber = tableNumer;
        GameManager.Instance.facebookManager.startRandomGame();
    }

    public void showFacebookFriends()
    {

        //AdsManager.Instance.adsScript.ShowAd(AdLocation.FacebookFriends);
        GameManager.Instance.playfabManager.GetPlayfabFriends();
    }

    public void setTableNumber()
    {
        GameManager.Instance.tableNumber = Int32.Parse(GameObject.Find("TextTableNumber").GetComponent<Text>().text);
    }


    public void ShowRewardedAd()
    {
        //#if UNITY_ANDROID || UNITY_IOS
        //        if (Advertisement.IsReady("rewardedVideo"))
        //        {
        //            var options = new ShowOptions { resultCallback = HandleShowResult };
        //            Advertisement.Show("rewardedVideo", options);
        //        }
        //#endif
    }

    public void Logout()
    {
        SceneManager.LoadScene("LoginSplash");
    }


    //#if UNITY_ANDROID || UNITY_IOS
    //    private void HandleShowResult(ShowResult result)
    //    {
    //        switch (result)
    //        {
    //            case ShowResult.Finished:
    //                Debug.Log("The ad was successfully shown.");
    //                GameManager.Instance.playfabManager.addCoinsRequest(StaticStrings.rewardForVideoAd);
    //                //
    //                // YOUR CODE TO REWARD THE GAMER
    //                // Give coins etc.
    //                break;
    //            case ShowResult.Skipped:
    //                Debug.Log("The ad was skipped before reaching the end.");
    //                break;
    //            case ShowResult.Failed:
    //                Debug.LogError("The ad failed to be shown.");
    //                break;
    //        }
    //    }
    //#endif

}
