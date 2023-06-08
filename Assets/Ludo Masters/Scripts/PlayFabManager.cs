using UnityEngine;
using System.Collections;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine.SceneManagement;
//using Facebook.Unity;
using System.Collections.Generic;
//using ExitGames.Client.Photon.Chat;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using AssemblyCSharp;
using System.Globalization;
using Photon.Realtime;
using PhotonPlayer = Photon.Realtime.Player;


public class PlayFabManager : MonoBehaviourPunCallbacks, IChatClientListener
{

    private Sprite[] avatarSprites;

    public string PlayFabId;
    public string authToken;
    public bool multiGame = true;
    public bool roomOwner = false;
    private FacebookManager fbManager;
    public GameObject fbButton;
    private FacebookFriendsMenu facebookFriendsMenu;
    public ChatClient chatClient;
    private bool alreadyGotFriends = false;
    public GameObject menuCanvas;
    public GameObject MatchPlayersCanvas;
    public GameObject splashCanvas;
    public bool opponentReady = false;
    public bool imReady = false;
    public GameObject playerAvatar;
    public GameObject playerName;
    public GameObject backButtonMatchPlayers;


    public GameObject loginEmail;
    public GameObject loginPassword;
    public GameObject loginInvalidEmailorPassword;
    public GameObject loginCanvas;


    public GameObject regiterEmail;
    public GameObject registerPassword;
    public GameObject registerNickname;
    public GameObject registerInvalidInput;
    public GameObject registerCanvas;

    public GameObject resetPasswordEmail;
    public GameObject resetPasswordInformationText;

    public bool isInLobby = false;
    public bool isInMaster = false;
    void Awake()
    {
        Debug.Log("Playfab awake");
        // PlayerPrefs.DeleteAll();
        //PhotonNetwork.PhotonServerSettings.HostType = ServerSettings.HostingOption.PhotonCloud;
        //PhotonNetwork.PhotonServerSettings.PreferredRegion = CloudRegionCode.eu;
        // PhotonNetwork.PhotonServerSettings.HostType = ServerSettings.HostingOption.BestRegion;
        // PhotonNetwork.PhotonServerSettings.AppID = StaticStrings.PhotonAppID;
        //#if UNITY_IOS
        //        PhotonNetwork.PhotonServerSettings.Protocol = ConnectionProtocol.Tcp;
        //#else
        //        PhotonNetwork.PhotonServerSettings.Protocol = ConnectionProtocol.Udp;
        //#endif
        //Debug.Log("PORT: " + PhotonNetwork.PhotonServerSettings.ServerPort);

        PlayFabSettings.TitleId = StaticStrings.PlayFabTitleID;

        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

        //PhotonNetwork. += this.OnEvent;
        DontDestroyOnLoad(transform.gameObject);
    }

    void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        //PhotonNetwork.OnEventCall -= this.OnEvent;
    }

    public void destroy()
    {
        if (this.gameObject != null)
            DestroyImmediate(this.gameObject);
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log("Playfab start");
        //PhotonNetwork.BackgroundTimeout = StaticStrings.photonDisconnectTimeoutLong; ;
        GameManager.Instance.playfabManager = this;
        fbManager = GameObject.Find("FacebookManager").GetComponent<FacebookManager>();
        facebookFriendsMenu = GameManager.Instance.facebookFriendsMenu;

        avatarSprites = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().avatars;
    }

    void Update()
    {
        if (chatClient != null) { chatClient.Service(); }
    }

    // handle events:
    public void OnEvent(EventData eventData)
    {

        Common.ScreenLog.ForceLog("Received event: " + (int)eventData.Code + " Sender ID: " + eventData.Sender);

        if (eventData.Code == (int)EnumPhoton.BeginPrivateGame)
        {
            //StartGame();
            LoadGameScene();
        }
        else if (eventData.Code == (int)EnumPhoton.StartWithBots && eventData.Sender != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            LoadBots();
        }
        else if (eventData.Code == (int)EnumPhoton.StartGame)
        {
            //Invoke("LoadGameWithDelay", UnityEngine.Random.Range(1.0f, 5.0f));
            //PhotonNetwork.LeaveRoom();
            LoadGameScene();
        }
        else if (eventData.Code == (int)EnumPhoton.ReadyToPlay)
        {
            GameManager.Instance.readyPlayersCount++;
            //LoadGameScene();
        }

    }

    //// handle events:
    //private void OnEvent(byte eventcode, object content, int senderid)
    //{

    //    Debug.Log("Received event: " + (int)eventcode + " Sender ID: " + senderid);

    //    if (eventcode == (int)EnumPhoton.BeginPrivateGame)
    //    {
    //        //StartGame();
    //        LoadGameScene();
    //    }
    //    else if (eventcode == (int)EnumPhoton.StartWithBots && senderid != PhotonNetwork.LocalPlayer.ActorNumber)
    //    {
    //        LoadBots();
    //    }
    //    else if (eventcode == (int)EnumPhoton.StartGame)
    //    {
    //        //Invoke("LoadGameWithDelay", UnityEngine.Random.Range(1.0f, 5.0f));
    //        //PhotonNetwork.LeaveRoom();
    //        LoadGameScene();
    //    }
    //    else if (eventcode == (int)EnumPhoton.ReadyToPlay)
    //    {
    //        GameManager.Instance.readyPlayersCount++;
    //        //LoadGameScene();
    //    }

    //}

    public void LoadGameWithDelay()
    {
        LoadGameScene();
    }

    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        if (GameManager.Instance.controlAvatars != null && GameManager.Instance.type == MyGameType.Private)
        {
            PhotonNetwork.LeaveRoom();
            GameManager.Instance.controlAvatars.ShowJoinFailed("Room closed");
        }
        else
        {
            if (newMasterClient.NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                Debug.Log("Im new master client");
                WaitForNewPlayer();
            }
        }

    }



    public void StartGame()
    {
        // while (!opponentReady || !imReady /*|| (!GameManager.Instance.roomOwner && !GameManager.Instance.receivedInitPositions)*/)
        // {
        //     yield return 0;
        // }

        Common.ScreenLog.ForceLog("StartGame");

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        CancelInvoke("StartGameWithBots");

        Invoke("startGameScene", 3.0f);

        //startGameScene();
    }

    private IEnumerator waitAndStartGame()
    {
        // while (!opponentReady || !imReady /*|| (!GameManager.Instance.roomOwner && !GameManager.Instance.receivedInitPositions)*/)
        // {
        //     yield return 0;
        // }
        while (GameManager.Instance.readyPlayers < GameManager.Instance.requiredPlayers - 1 || !imReady /*|| (!GameManager.Instance.roomOwner && !GameManager.Instance.receivedInitPositions)*/)
        {
            yield return 0;
        }
        startGameScene();
        GameManager.Instance.readyPlayers = 0;
        opponentReady = false;
        imReady = false;
    }

    public void startGameScene()
    {
        if (GameManager.Instance.currentPlayersCount >= GameManager.Instance.requiredPlayers || GameManager.Instance.type == MyGameType.Private)
        {
            //call genie api to create a room with opponent ids and user id..
            Common.ScreenLog.ForceLog("startGameScene");
            LoadGameScene();

            RaiseEventOptions raiseEventOptions = RaiseEventOptions.Default;

            SendOptions sendOptions = new SendOptions();
            sendOptions.Reliability = true;

            if (GameManager.Instance.type == MyGameType.Private)
            {
                PhotonNetwork.RaiseEvent((int)EnumPhoton.BeginPrivateGame, null, raiseEventOptions, sendOptions);
                Common.ScreenLog.ForceLog("Raise private game");
            }
            else
            {
                PhotonNetwork.RaiseEvent((int)EnumPhoton.StartGame, null, raiseEventOptions, sendOptions);
                Common.ScreenLog.ForceLog("Raise public game");
            }

        }
        else
        {
            Common.ScreenLog.ForceLog("Waiting for player");
            if (PhotonNetwork.IsMasterClient)
                WaitForNewPlayer();
        }
    }


    public void LoadGameScene()
    {
        GameManager.Instance.GameScene = "GameScene";

        if (!GameManager.Instance.gameSceneStarted)
        {
            SceneManager.LoadScene(GameManager.Instance.GameScene);
            GameManager.Instance.gameSceneStarted = true;
        }

    }



    public void WaitForNewPlayer()
    {
        if (PhotonNetwork.IsMasterClient && GameManager.Instance.type != MyGameType.Private)
        {
            Debug.Log("START INVOKE");
            CancelInvoke("StartGameWithBots");
            //Bots removed
            //Invoke("StartGameWithBots", StaticStrings.WaitTimeUntilStartWithBots);
        }
    }

    public void StartGameWithBots()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount < GameManager.Instance.requiredPlayers)
            {
                Debug.Log("Master Client");
                // PhotonNetwork.RaiseEvent((int)EnumPhoton.StartWithBots, null, true, null);
                LoadBots();
            }
        }
        else
        {
            Debug.Log("Not Master client");
        }
    }

    public void LoadBots()
    {
        Debug.Log("Close room - add bots");
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        if (PhotonNetwork.IsMasterClient)
        {
            Invoke("AddBots", 3.0f);
        }
        else
        {
            AddBots();
        }

    }

    public void AddBots()
    {
        // Add Bots here

        Debug.Log("Add Bots with delay");

        if (PhotonNetwork.CurrentRoom.PlayerCount < GameManager.Instance.requiredPlayers)
        {

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.RaiseEvent((int)EnumPhoton.StartWithBots, null, new RaiseEventOptions(), new SendOptions() { Reliability = true });
            }

            for (int i = 0; i < GameManager.Instance.requiredPlayers - 1; i++)
            {
                if (GameManager.Instance.opponentsIDs[i] == null)
                {
                    StartCoroutine(AddBot(i));
                }
            }
        }
    }


    public IEnumerator AddBot(int i)
    {
        yield return new WaitForSeconds(i + UnityEngine.Random.Range(0.0f, 0.9f));

        GameManager.Instance.opponentsAvatars[i] = avatarSprites[UnityEngine.Random.Range(0, avatarSprites.Length - 1)];
        GameManager.Instance.opponentsIDs[i] = "_BOT" + i;
        GameManager.Instance.opponentsNames[i] = "Guest" + UnityEngine.Random.Range(100000, 999999);
        Debug.Log("Name: " + GameManager.Instance.opponentsNames[i]);
        GameManager.Instance.controlAvatars.PlayerJoined(i, "_BOT" + i);
    }

    public void resetPassword()
    {
        resetPasswordInformationText.SetActive(false);

        SendAccountRecoveryEmailRequest request = new SendAccountRecoveryEmailRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            Email = resetPasswordEmail.GetComponent<Text>().text
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(request, (result) =>
        {
            resetPasswordInformationText.SetActive(true);
            resetPasswordInformationText.GetComponent<Text>().text = "Email sent to your address. Check your inbox";


        }, (error) =>
        {
            resetPasswordInformationText.SetActive(true);
            resetPasswordInformationText.GetComponent<Text>().text = "Account with specified email doesn't exist";
        });
    }

    public void setInitNewAccountData(bool fb)
    {
        Dictionary<string, string> data = MyPlayerData.InitialUserData(fb);
        GameManager.Instance.myPlayerData.UpdateUserData(data);
    }


    public void updateBoughtChats(int index)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add(MyPlayerData.ChatsKey, GameManager.Instance.myPlayerData.GetChats() + ";'" + index + "'");


        GameManager.Instance.myPlayerData.UpdateUserData(data);


    }

    public void UpdateBoughtEmojis(int index)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add(MyPlayerData.EmojiKey, GameManager.Instance.myPlayerData.GetEmoji() + ";'" + index + "'");


        GameManager.Instance.myPlayerData.UpdateUserData(data);
    }

    public void addCoinsRequest(float count)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add(MyPlayerData.CoinsKey, "" + (GameManager.Instance.myPlayerData.GetCoins() + count));
        GameManager.Instance.myPlayerData.UpdateUserData(data);
    }

    public void getPlayerDataRequest()
    {
        Debug.Log("Get player data request!!");

        GetUserDataRequest getdatarequest = new GetUserDataRequest()
        {
            PlayFabId = GameManager.Instance.playfabManager.PlayFabId,
        };

        PlayFabClientAPI.GetUserData(getdatarequest, (result) =>
        {

            Dictionary<string, UserDataRecord> data = result.Data;

            GameManager.Instance.myPlayerData = new MyPlayerData(data, true);


            Debug.Log("Get player data request finish!!");

            //StartCoroutine(loadSceneMenu());
            SceneManager.LoadScene("MenuScene");

        }, (error) =>
        {
            Debug.Log("Data updated error " + error.ErrorMessage);
        }, null);
    }


    private IEnumerator loadSceneMenu()
    {
        yield return new WaitForSeconds(0.5f);

        if (isInMaster && isInLobby)
        {
            SceneManager.LoadScene("MenuScene");
            Debug.Log("SceneLoaded");
        }
        else
        {
            StartCoroutine(loadSceneMenu());
        }

    }

    public void RegisterNewAccountWithID()
    {
        string email = regiterEmail.GetComponent<Text>().text;
        string password = registerPassword.GetComponent<Text>().text;
        string nickname = registerNickname.GetComponent<Text>().text;

        registerInvalidInput.SetActive(false);

        if (Regex.IsMatch(email, @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$") && password.Length >= 6 && nickname.Length > 0)
        {



            RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                Email = email,
                Password = password,
                RequireBothUsernameAndEmail = false
            };

            PlayFabClientAPI.RegisterPlayFabUser(request, (result) =>
            {

                PlayFabId = result.PlayFabId;
                Debug.Log("Got PlayFabID: " + PlayFabId);

                registerCanvas.SetActive(false);
                PlayerPrefs.SetString("email_account", email);
                PlayerPrefs.SetString("password", password);
                PlayerPrefs.SetString("LoggedType", "EmailAccount");
                PlayerPrefs.Save();
                GameManager.Instance.nameMy = nickname;


                setInitNewAccountData(false);


                UpdateUserTitleDisplayNameRequest displayNameRequest = new UpdateUserTitleDisplayNameRequest()
                {
                    DisplayName = GameManager.Instance.playfabManager.PlayFabId
                };

                PlayFabClientAPI.UpdateUserTitleDisplayName(displayNameRequest, (response) =>
                {
                    Debug.Log("Title Display name updated successfully");
                }, (error) =>
                {
                    Debug.Log("Title Display name updated error: " + error.Error);

                }, null);


                Dictionary<string, string> data = new Dictionary<string, string>();

                data.Add("LoggedType", "EmailAccount");
                data.Add("PlayerName", GameManager.Instance.nameMy);

                GameManager.Instance.myPlayerData.UpdateUserData(data);

                fbManager.showLoadingCanvas();
                GetPhotonToken();

            },
                (error) =>
                {
                    registerInvalidInput.SetActive(true);
                    registerInvalidInput.GetComponent<Text>().text = error.ErrorMessage;
                    Debug.Log("Error registering new account with email: " + error.ErrorMessage + "\n" + error.ErrorDetails);
                });
        }
        else
        {
            registerInvalidInput.SetActive(true);
            registerInvalidInput.GetComponent<Text>().text = "Invalid input specified";
        }


    }

    public void LinkFacebookAccount()
    {
        LinkFacebookAccountRequest request = new LinkFacebookAccountRequest()
        {
            //AccessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString,
            ForceLink = true
        };

        PlayFabClientAPI.LinkFacebookAccount(request, (result) =>
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("LoggedType", "Facebook");
            //data.Add("FacebookID", Facebook.Unity.AccessToken.CurrentAccessToken.UserId);
            data.Add("PlayerAvatarUrl", GameManager.Instance.avatarMyUrl);
            data.Add(MyPlayerData.PlayerName, GameManager.Instance.nameMy);
            data.Add(MyPlayerData.AvatarIndexKey, "fb");
            data.Add(MyPlayerData.CoinsKey, (GameManager.Instance.myPlayerData.GetCoins() + StaticStrings.CoinsForLinkToFacebook).ToString());
            GameManager.Instance.myAvatarGameObject.GetComponent<Image>().sprite = GameManager.Instance.facebookAvatar;
            GameManager.Instance.myNameGameObject.GetComponent<Text>().text = GameManager.Instance.nameMy;
            GameManager.Instance.myPlayerData.UpdateUserData(data);

            GameManager.Instance.FacebookLinkButton.SetActive(false);
        },
        (error) =>
        {
            Debug.Log("Error linking facebook account: " + error.ErrorMessage + "\n" + error.ErrorDetails);
            GameManager.Instance.connectionLost.showDialog();
        });



    }

    public void LoginWithFacebook()
    {
        LoginWithFacebookRequest request = new LoginWithFacebookRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            CreateAccount = true,
            //AccessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString
        };

        PlayFabClientAPI.LoginWithFacebook(request, (result) =>
        {
            PlayFabId = result.PlayFabId;
            Debug.Log("Got PlayFabID: " + PlayFabId);



            if (result.NewlyCreated)
            {
                Debug.Log("(new account)");
                setInitNewAccountData(true);
                Dictionary<string, string> data1 = new Dictionary<string, string>();
                data1.Add(MyPlayerData.AvatarIndexKey, "fb");
                GameManager.Instance.myPlayerData.UpdateUserData(data1);
            }
            else
            {
                CheckIfFirstTitleLogin(PlayFabId, true);
                Debug.Log("(existing account)");
            }


            UpdateUserTitleDisplayNameRequest displayNameRequest = new UpdateUserTitleDisplayNameRequest()
            {
                DisplayName = GameManager.Instance.playfabManager.PlayFabId
            };

            PlayFabClientAPI.UpdateUserTitleDisplayName(displayNameRequest, (response) =>
            {
                Debug.Log("Title Display name updated successfully");
            }, (error) =>
            {
                Debug.Log("Title Display name updated error: " + error.Error);

            }, null);


            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("LoggedType", "Facebook");
            //data.Add("FacebookID", Facebook.Unity.AccessToken.CurrentAccessToken.UserId);
            if (result.NewlyCreated)
                data.Add("PlayerName", GameManager.Instance.nameMy);
            else
            {
                GetUserDataRequest getdatarequest = new GetUserDataRequest()
                {
                    PlayFabId = result.PlayFabId,

                };

                PlayFabClientAPI.GetUserData(getdatarequest, (result2) =>
                {

                    Dictionary<string, UserDataRecord> data2 = result2.Data;

                    if (data2.ContainsKey("PlayerName"))
                    {
                        GameManager.Instance.nameMy = data2["PlayerName"].Value;
                    }
                    else
                    {
                        Dictionary<string, string> data5 = new Dictionary<string, string>();
                        data5.Add("PlayerName", GameManager.Instance.nameMy);
                        data5.Add(MyPlayerData.AvatarIndexKey, "fb");
                        GameManager.Instance.myPlayerData.UpdateUserData(data5);
                    }
                }, (error) =>
                {
                    Debug.Log("Data updated error " + error.ErrorMessage);
                }, null);
            }
            data.Add("PlayerAvatarUrl", GameManager.Instance.avatarMyUrl);

            GameManager.Instance.myPlayerData.UpdateUserData(data);


            GetPhotonToken();

        },
            (error) =>
            {
                Debug.Log("Error logging in player with custom ID: " + error.ErrorMessage + "\n" + error.ErrorDetails);
                GameManager.Instance.connectionLost.showDialog();
            });
    }

    public void CheckIfFirstTitleLogin(string id, bool fb)
    {
        GetUserDataRequest getdatarequest = new GetUserDataRequest()
        {
            PlayFabId = id,

        };

        PlayFabClientAPI.GetUserData(getdatarequest, (result) =>
        {
            Dictionary<string, UserDataRecord> data = result.Data;

            if (!data.ContainsKey(MyPlayerData.TitleFirstLoginKey))
            {
                Debug.Log("First login for this title. Set initial data");
                setInitNewAccountData(fb);
            }

        }, (error) =>
        {
            Debug.Log("Data updated error " + error.ErrorMessage);
        }, null);
    }

    private string androidUnique()
    {
        AndroidJavaClass androidUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityPlayerActivity = androidUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject unityPlayerResolver = unityPlayerActivity.Call<AndroidJavaObject>("getContentResolver");
        AndroidJavaClass androidSettingsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
        return androidSettingsSecure.CallStatic<string>("getString", unityPlayerResolver, "android_id");
    }

    public void LoginWithEmailAccount()
    {
        Common.ScreenLog.ForceLog("Player login email");

        loginInvalidEmailorPassword.SetActive(false);



        string email = "";
        string password = "";
        if (PlayerPrefs.HasKey("email_account"))
        {
            email = PlayerPrefs.GetString("email_account");
            password = PlayerPrefs.GetString("password");
        }
        else
        {
            email = loginEmail.GetComponent<Text>().text;
            password = loginPassword.GetComponent<Text>().text;

        }


        LoginWithEmailAddressRequest request = new LoginWithEmailAddressRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            Email = email,
            Password = password
        };

        Common.ScreenLog.ForceLog("Playfab login init");

        PlayFabClientAPI.LoginWithEmailAddress(request, (result) =>
        {
            PlayFabId = result.PlayFabId;
            Debug.Log("Got PlayFabID: " + PlayFabId);
            Common.ScreenLog.ForceLog("Playfab login success");

            Dictionary<string, string> data = new Dictionary<string, string>();

            loginCanvas.SetActive(false);
            PlayerPrefs.SetString("email_account", email);
            PlayerPrefs.SetString("password", password);
            PlayerPrefs.SetString("LoggedType", "EmailAccount");
            PlayerPrefs.Save();



            if (result.NewlyCreated)
            {
                Debug.Log("(new account)");
                setInitNewAccountData(false);
                //addCoinsRequest(StaticStrings.initCoinsCount);
            }
            else
            {
                CheckIfFirstTitleLogin(PlayFabId, false);
                Debug.Log("(existing account)");
            }


            GetUserDataRequest getdatarequest = new GetUserDataRequest()
            {
                PlayFabId = result.PlayFabId,

            };

            PlayFabClientAPI.GetUserData(getdatarequest, (result2) =>
            {

                Dictionary<string, UserDataRecord> data2 = result2.Data;

                if (data2.ContainsKey("PlayerName"))
                {
                    GameManager.Instance.nameMy = data2["PlayerName"].Value;
                }
                else
                {
                    Dictionary<string, string> data5 = new Dictionary<string, string>();
                    data5.Add("PlayerName", GameManager.Instance.nameMy);
                    GameManager.Instance.myPlayerData.UpdateUserData(data5);
                }
                GameManager.Instance.nameMy = data2["PlayerName"].Value;
            }, (error) =>
            {
                Debug.Log("Data updated error " + error.ErrorMessage);
            }, null);





            fbManager.showLoadingCanvas();


            GetPhotonToken();

        },
             (error) =>
             {
                 Common.ScreenLog.ForceLog("Playfab login error: " + error.ErrorMessage);
                 loginInvalidEmailorPassword.SetActive(true);
                 Debug.Log("Error logging in player with custom ID: " + error.ErrorMessage);
                 //Debug.Log(error.ErrorMessage);

                 //GameManager.Instance.connectionLost.showDialog();
             });
    }

    public void Login()
    {
        Common.ScreenLog.ForceLog("Player login");

        string customId = "";
        if (PlayerPrefs.HasKey("unique_identifier"))
        {
            customId = PlayerPrefs.GetString("unique_identifier");
        }
        else
        {
            customId = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("unique_identifier", customId);
        }




        Debug.Log("UNIQUE IDENTIFIER: " + customId);

        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            CreateAccount = true,
            CustomId = customId //SystemInfo.deviceUniqueIdentifier
        };


        Common.ScreenLog.ForceLog("Playfab login init");

        PlayFabClientAPI.LoginWithCustomID(request, (result) =>
        {
            PlayFabId = result.PlayFabId;
            Debug.Log("Got PlayFabID: " + PlayFabId);

            Common.ScreenLog.ForceLog("Playfab login success");

            Dictionary<string, string> data = new Dictionary<string, string>();

            if (result.NewlyCreated)
            {
                Debug.Log("(new account)");
                setInitNewAccountData(false);

                string name = result.PlayFabId;
                name = "Guest";
                for (int i = 0; i < 6; i++)
                {
                    name += UnityEngine.Random.Range(0, 9);
                }

                data.Add("PlayerName", Ludo.IN.GameManager.Instance.nameMy);
                //addCoinsRequest(StaticStrings.initCoinsCount);
            }
            else
            {
                CheckIfFirstTitleLogin(PlayFabId, false);
                Debug.Log("(existing account)");
            }



            // string name = result.PlayFabId;
            // if (PlayerPrefs.HasKey("GuestPlayerName"))
            // {
            //     //name = PlayerPrefs.GetString("GuestPlayerName");
            // }
            // else
            // {
            //     name = "Guest";
            //     for (int i = 0; i < 6; i++)
            //     {
            //         name += UnityEngine.Random.Range(0, 9);
            //     }
            //     PlayerPrefs.SetString("GuestPlayerName", name);
            //     PlayerPrefs.Save();
            //     data.Add("PlayerName", name);
            // }


            data.Add("LoggedType", "Guest");



            UpdateUserTitleDisplayNameRequest displayNameRequest = new UpdateUserTitleDisplayNameRequest()
            {
                //DisplayName = name
                DisplayName = GameManager.Instance.playfabManager.PlayFabId
            };

            PlayFabClientAPI.UpdateUserTitleDisplayName(displayNameRequest, (response) =>
            {
                Debug.Log("Title Display name updated successfully");
                Common.ScreenLog.ForceLog("Title Display name updated success 0");
            }, (error) =>
            {
                Debug.Log("Title Display name updated error: " + error.Error);
                Common.ScreenLog.ForceLog("Title Display name updated error 0: " + error.ErrorMessage);

            }, null);


            GameManager.Instance.myPlayerData.UpdateUserData(data);

            //set genie account name
            //GameManager.Instance.nameMy = name;
            GameManager.Instance.nameMy = Ludo.IN.GameManager.Instance.nameMy;

            PlayerPrefs.SetString("LoggedType", "Guest");
            PlayerPrefs.Save();

            //fbManager.showLoadingCanvas();

            RequestPhotonToken(result);
            // GetPhotonToken();


        },
            (error) =>
            {
                Common.ScreenLog.ForceLog("Playfab login error: " + error.ErrorMessage);
                Debug.Log("Error logging in player with custom ID:");
                Debug.Log(error.ErrorMessage);
                GameManager.Instance.connectionLost.showDialog();
            });
    }

    public void GetPlayfabFriends()
    {
        if (alreadyGotFriends)
        {
            Debug.Log("show firneds FFFF");
            if (PlayerPrefs.GetString("LoggedType").Equals("Facebook"))
            {
                //fbManager.getFacebookInvitableFriends();
            }
            else
            {

                facebookFriendsMenu.showFriends(null, null, null);
            }
        }
        else
        {
            Debug.Log("IND");
            GetFriendsListRequest request = new GetFriendsListRequest();
            request.IncludeFacebookFriends = true;
            PlayFabClientAPI.GetFriendsList(request, (result) =>
            {

                Debug.Log("Friends list Playfab: " + result.Friends.Count);
                var friends = result.Friends;

                List<string> playfabFriends = new List<string>();
                List<string> playfabFriendsName = new List<string>();
                List<string> playfabFriendsFacebookId = new List<string>();


                chatClient.RemoveFriends(GameManager.Instance.friendsIDForStatus.ToArray());

                List<string> friendsToStatus = new List<string>();


                int index = 0;
                foreach (var friend in friends)
                {

                    playfabFriends.Add(friend.FriendPlayFabId);

                    Debug.Log("Title: " + friend.TitleDisplayName);
                    GetUserDataRequest getdatarequest = new GetUserDataRequest()
                    {
                        PlayFabId = friend.TitleDisplayName,
                    };


                    int ind2 = index;

                    PlayFabClientAPI.GetUserData(getdatarequest, (result2) =>
                    {

                        Dictionary<string, UserDataRecord> data2 = result2.Data;
                        playfabFriendsName[ind2] = data2["PlayerName"].Value;
                        Debug.Log("Added " + data2["PlayerName"].Value);
                        GameManager.Instance.facebookFriendsMenu.updateName(ind2, data2["PlayerName"].Value, friend.TitleDisplayName);

                    }, (error) =>
                    {

                        Debug.Log("Data updated error " + error.ErrorMessage);
                    }, null);

                    playfabFriendsName.Add("");

                    friendsToStatus.Add(friend.FriendPlayFabId);

                    index++;
                }

                GameManager.Instance.friendsIDForStatus = friendsToStatus;

                chatClient.AddFriends(friendsToStatus.ToArray());

                GameManager.Instance.facebookFriendsMenu.addPlayFabFriends(playfabFriends, playfabFriendsName, playfabFriendsFacebookId);

                if (PlayerPrefs.GetString("LoggedType").Equals("Facebook"))
                {
                    //fbManager.getFacebookInvitableFriends();
                }
                else
                {
                    GameManager.Instance.facebookFriendsMenu.showFriends(null, null, null);
                }
            }, OnPlayFabError);
        }


    }


    void OnPlayFabError(PlayFabError error)
    {
        Debug.Log("Playfab Error: " + error.ErrorMessage);
    }

    // #######################  PHOTON  ##########################

    void GetPhotonToken()
    {

        GetPhotonAuthenticationTokenRequest request = new GetPhotonAuthenticationTokenRequest();
        request.PhotonApplicationId = StaticStrings.PhotonAppID.Trim();

        PlayFabClientAPI.GetPhotonAuthenticationToken(request, OnPhotonAuthenticationSuccess, OnPlayFabError);

        Debug.Log(request);
    }


    private void RequestPhotonToken(PlayFab.ClientModels.LoginResult obj)
    {
        Debug.Log("PlayFab authenticated. Requesting photon token...");
        //We can player PlayFabId. This will come in handy during next step
        string _playFabPlayerIdCache = obj.PlayFabId;

        Common.ScreenLog.ForceLog("Photon authenticating");

        PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest()
        {
            PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime
        }, OnPhotonAuthenticationSuccess, OnPlayFabError);
    }


    void OnPhotonAuthenticationSuccess(GetPhotonAuthenticationTokenResult result)
    {
        string photonToken = result.PhotonCustomAuthenticationToken;
        Debug.Log(string.Format("Yay, logged in session token: {0}", photonToken));
        PhotonNetwork.AuthValues = new Photon.Realtime.AuthenticationValues();
        PhotonNetwork.AuthValues.AuthType = Photon.Realtime.CustomAuthenticationType.Custom;

        Common.ScreenLog.ForceLog("Photon authentication success");

        //Playfab References
        PhotonNetwork.AuthValues.AddAuthParameter("username", this.PlayFabId);
        PhotonNetwork.AuthValues.AddAuthParameter("Token", result.PhotonCustomAuthenticationToken);
        PhotonNetwork.AuthValues.UserId = this.PlayFabId;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.LocalPlayer.NickName = this.PlayFabId;
        authToken = result.PhotonCustomAuthenticationToken;
        getPlayerDataRequest();
        connectToChat();

    }

    public void connectToChat()
    {
        chatClient = new ChatClient(this);
        GameManager.Instance.chatClient = chatClient;
        Photon.Chat.AuthenticationValues authValues = new Photon.Chat.AuthenticationValues();
        authValues.UserId = this.PlayFabId;
        authValues.AuthType = Photon.Chat.CustomAuthenticationType.Custom;
        authValues.AddAuthParameter("username", this.PlayFabId);
        authValues.AddAuthParameter("Token", authToken);
        chatClient.Connect(StaticStrings.PhotonChatID, "1.4", authValues);
    }


    //public override void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    //{

    //    Debug.Log("Custom properties changed: " + DateTime.Now.ToString());
    //}


    public override void OnConnected()
    {
        Debug.Log("Photon Chat connected!!!");
        chatClient.Subscribe(new string[] { "invitationsChannel" });
    }

    public override void OnPlayerLeftRoom(PhotonPlayer player)
    {
        GameManager.Instance.opponentDisconnected = true;

        GameManager.Instance.invitationID = "";

        if (GameManager.Instance.controlAvatars != null)
        {
            Debug.Log("PLAYER DISCONNECTED " + player.NickName);
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                GameManager.Instance.controlAvatars.startButtonPrivate.GetComponent<Button>().interactable = true;
            }
            else
            {
                GameManager.Instance.controlAvatars.startButtonPrivate.GetComponent<Button>().interactable = false;
            }


            int index = GameManager.Instance.opponentsIDs.IndexOf(player.NickName);
            //PhotonNetwork.room.IsOpen = true;
            GameManager.Instance.controlAvatars.PlayerDisconnected(index);
        }
    }

    public void showMenu()
    {
        menuCanvas.gameObject.SetActive(true);

        playerName.GetComponent<Text>().text = GameManager.Instance.nameMy;

        if (GameManager.Instance.avatarMy != null)
            playerAvatar.GetComponent<Image>().sprite = GameManager.Instance.avatarMy;

        splashCanvas.SetActive(false);
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("Subscribed to CHAT - set online status!");
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }


    public void challengeFriend(string id, string message)
    {
        //if (GameManager.Instance.invitationID.Length == 0 || !GameManager.Instance.invitationID.Equals(id))
        //{
        chatClient.SendPrivateMessage(id, "INVITE_TO_PLAY_PRIVATE;" + /*id + this.PlayFabId + ";" +*/ GameManager.Instance.nameMy + ";" + message);
        GameManager.Instance.invitationID = id;
        Debug.Log("Send invitation to: " + id);
        // }
    }

    string roomname;
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        if (!sender.Equals(this.PlayFabId))
        {
            if (message.ToString().Contains("INVITE_TO_PLAY_PRIVATE"))
            {
                GameManager.Instance.invitationID = sender;

                string[] messageSplit = message.ToString().Split(';');
                string whoInvite = messageSplit[1];
                string payout = messageSplit[2];
                string roomID = messageSplit[3];
                GameManager.Instance.payoutCoins = int.Parse(payout);
                GameManager.Instance.invitationDialog.GetComponent<PhotonChatListener>().showInvitationDialog(0, whoInvite, payout, roomID, 0);
            }
        }

        if ((GameManager.Instance.invitationID.Length == 0 || !GameManager.Instance.invitationID.Equals(sender)))
        {

        }
        else
        {
            GameManager.Instance.invitationID = "";
        }
    }

    public void join()
    {
        PhotonNetwork.JoinRoom(roomname);
    }

    public void DebugReturn(DebugLevel level, string message)
    {

    }

    public void OnChatStateChange(ChatState state)
    {

    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from photon");
        backToMenu();
    }

    public void DisconnecteFromPhoton()
    {
        PhotonNetwork.Disconnect();
    }

    public void backToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void switchUser()
    {
        GameManager.Instance.playfabManager.destroy();
        GameManager.Instance.facebookManager.destroy();
        GameManager.Instance.connectionLost.destroy();
        //GameManager.Instance.adsScript.destroy();
        GameManager.Instance.avatarMy = null;
        GameManager.Instance.logged = false;
        GameManager.Instance.resetAllData();
        SceneManager.LoadScene("LoginSplash");
    }

    public void OnDisconnected()
    {
        //Debug.Log("Chat disconnected - Reconnect");
        connectToChat();
    }



    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {

    }

    public void OnUnsubscribed(string[] channels)
    {

    }


    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("STATUS UPDATE CHAT!");
        Debug.Log("Status change for: " + user + " to: " + status);

        bool foundFriend = false;
        for (int i = 0; i < GameManager.Instance.friendsStatuses.Count; i++)
        {
            string[] friend = GameManager.Instance.friendsStatuses[i];
            if (friend[0].Equals(user))
            {
                GameManager.Instance.friendsStatuses[i][1] = "" + status;
                foundFriend = true;
                break;
            }
        }

        if (!foundFriend)
        {
            GameManager.Instance.friendsStatuses.Add(new string[] { user, "" + status });
        }

        if (GameManager.Instance.facebookFriendsMenu != null)
            GameManager.Instance.facebookFriendsMenu.updateFriendStatus(status, user);
    }

    public override void OnConnectedToMaster()
    {
        isInMaster = true;
        Common.ScreenLog.ForceLog("Connected to master");

        PhotonNetwork.JoinLobby();

    }
    public void LeaveLobby()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            isInMaster = false;
        }
    }

    public override void OnJoinedLobby()
    {
        Common.ScreenLog.ForceLog("joined lobby");
        isInLobby = true;
    }

    public void JoinRoomAndStartGame()
    {
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {
            {"m", GameManager.Instance.mode.ToString() +  GameManager.Instance.type.ToString() + GameManager.Instance.payoutCoins.ToString()}
         };

        StartCoroutine(TryToJoinRandomRoom(expectedCustomRoomProperties));

        //PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public IEnumerator TryToJoinRandomRoom(ExitGames.Client.Photon.Hashtable roomOptions)
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.ConnectUsingSettings();
            Common.ScreenLog.ForceLog("Connecting to photon");
            yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);
            yield return new WaitForEndOfFrame();
            Common.ScreenLog.ForceLog("Connected to photon");
        }

        if(TestingMode.freeMatches)
        {
            Ludo.IN.GameManager.Instance.Genie_Curr_roomId = 19;
        }
        else
        {
            switch (GameManager.Instance.type)
            {
                case MyGameType.TwoPlayer:
                    Ludo.IN.GameManager.Instance.Genie_Curr_roomId = 18;
                    break;
                case MyGameType.FourPlayer:
                    Ludo.IN.GameManager.Instance.Genie_Curr_roomId = 20;
                    break;
            }
        }

        PhotonNetwork.JoinRandomRoom(roomOptions, 0);

        //while (true)
        //{
        //    if (isInLobby && isInMaster)
        //    {
        //        if (GameManager.Instance.type == MyGameType.TwoPlayer)
        //        {
        //            //Ludo.IN.GameManager.Instance.Genie_Curr_roomId = 18;
        //            Ludo.IN.GameManager.Instance.Genie_Curr_roomId = 19;
        //        }
        //        else
        //        {
        //            if (GameManager.Instance.type == MyGameType.FourPlayer)
        //                //Ludo.IN.GameManager.Instance.Genie_Curr_roomId = 20;
        //                Ludo.IN.GameManager.Instance.Genie_Curr_roomId = 19;
        //        }

        //        PhotonNetwork.JoinRandomRoom(roomOptions, 0);
        //        break;
        //    }
        //    else
        //    {
        //        yield return new WaitForSeconds(0.05f);
        //    }
        //}
    }


    public void OnPhotonRandomJoinFailed()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomPropertiesForLobby = new String[] { "m", "v" };




        string BotMoves = generateBotMoves();

        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {
            { "m", GameManager.Instance.mode.ToString() +  GameManager.Instance.type.ToString() + GameManager.Instance.payoutCoins.ToString()},
            {"bt", BotMoves},
            {"fp", UnityEngine.Random.Range(0, GameManager.Instance.requiredPlayers)}
         };

        Debug.Log("Create Room: " + GameManager.Instance.mode.ToString() + GameManager.Instance.type.ToString() + GameManager.Instance.payoutCoins.ToString());
        roomOptions.MaxPlayers = (byte)GameManager.Instance.requiredPlayers;
        //roomOptions.IsVisible = true;

        StartCoroutine(TryToCreateGameAfterFailedToJoinRandom(roomOptions));

    }

    public string generateBotMoves()
    {
        // Generate BOT moves
        string BotMoves = "";
        int BotCount = 100;
        // Generate dice values
        for (int i = 0; i < BotCount; i++)
        {
            BotMoves += (UnityEngine.Random.Range(1, 7)).ToString();
            if (i < BotCount - 1)
            {
                BotMoves += ",";
            }
        }

        BotMoves += ";";

        // Generate delays
        float minValue = GameManager.Instance.playerTime / 10;
        if (minValue < 1.5f) minValue = 1.5f;
        for (int i = 0; i < BotCount; i++)
        {
            BotMoves += (UnityEngine.Random.Range(minValue, GameManager.Instance.playerTime / 8)).ToString();
            if (i < BotCount - 1)
            {
                BotMoves += ",";
            }
        }
        return BotMoves;
    }

    public void extractBotMoves(string data)
    {
        GameManager.Instance.botDiceValues = new List<int>();
        GameManager.Instance.botDelays = new List<float>();
        string[] d1 = data.Split(';');


        string[] diceValues = d1[0].Split(',');
        for (int i = 0; i < diceValues.Length; i++)
        {
            GameManager.Instance.botDiceValues.Add(int.Parse(diceValues[i]));
        }

        string[] delays = d1[1].Split(',');
        for (int i = 0; i < delays.Length; i++)
        {
            GameManager.Instance.botDelays.Add(float.Parse(delays[i]));
        }
    }

    public IEnumerator TryToCreateGameAfterFailedToJoinRandom(RoomOptions roomOptions)
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.ConnectUsingSettings();
            Common.ScreenLog.ForceLog("Connecting to photon");
            yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);
            yield return new WaitForEndOfFrame();
            Common.ScreenLog.ForceLog("Connected to photon");
        }

        while (true)
        {
            if (isInLobby && isInMaster)
            {
                PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);


                break;
            }
            else
            {
                yield return new WaitForSeconds(0.05f);
            }
        }
    }


    public override void OnJoinedRoom()
    {

        Common.ScreenLog.ForceLog("Room joined: " + PhotonNetwork.CurrentRoom.Name);
        GenieAPIClass.Instance.SendMyUserIDAndName();


        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("bt"))
        {
            extractBotMoves(PhotonNetwork.CurrentRoom.CustomProperties["bt"].ToString());
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("fp"))
        {
            GameManager.Instance.firstPlayerInGame = int.Parse(PhotonNetwork.CurrentRoom.CustomProperties["fp"].ToString());
        }
        else
        {
            GameManager.Instance.firstPlayerInGame = 0;
        }



        GameManager.Instance.avatarOpponent = null;

        Debug.Log("Players in room " + PhotonNetwork.CurrentRoom.PlayerCount);

        GameManager.Instance.currentPlayersCount = 1;

        GameManager.Instance.controlAvatars.setCancelButton();
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Common.ScreenLog.ForceLog("one player in room");
            GameManager.Instance.roomOwner = true;
            WaitForNewPlayer();
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount >= GameManager.Instance.requiredPlayers)
        {

            Common.ScreenLog.ForceLog("Two players in room");
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }

        if (!roomOwner)
        {
            //send local player data
            //GenieAPIClass.Instance.SendMyUserIDAndName();

            GameManager.Instance.backButtonMatchPlayers.SetActive(false);

            for (int i = 0; i < PhotonNetwork.PlayerListOthers.Length; i++)
            {

                int ii = i;
                int index = GetFirstFreeSlot();
                GameManager.Instance.opponentsIDs[index] = PhotonNetwork.PlayerListOthers[ii].NickName;

                GetUserDataRequest getdatarequest = new GetUserDataRequest()
                {
                    PlayFabId = PhotonNetwork.PlayerListOthers[ii].NickName,
                };

                string otherID = PhotonNetwork.PlayerListOthers[ii].NickName;


                PlayFabClientAPI.GetUserData(getdatarequest, (result) =>
                {
                    Dictionary<string, UserDataRecord> data = result.Data;

                    if (data.ContainsKey("LoggedType"))
                    {
                        if (data["LoggedType"].Value.Equals("Facebook"))
                        {
                            bool fbAvatar = true;
                            int avatarIndex = 0;
                            if (!data[MyPlayerData.AvatarIndexKey].Value.Equals("fb"))
                            {
                                fbAvatar = false;
                                avatarIndex = int.Parse(data[MyPlayerData.AvatarIndexKey].Value.ToString());
                            }

                            Common.ScreenLog.ForceLog("Getting opponent data");
                            getOpponentData(data, index, fbAvatar, avatarIndex, otherID);
                        }
                        else
                        {
                            if (data.ContainsKey("PlayerName"))
                            {
                                GameManager.Instance.opponentsNames[index] = data["PlayerName"].Value;
                                //GameManager.Instance.controlAvatars.PlayerJoined(index);
                                bool fbAvatar = true;
                                int avatarIndex = 0;
                                if (!data[MyPlayerData.AvatarIndexKey].Value.Equals("fb"))
                                {
                                    fbAvatar = false;
                                    avatarIndex = int.Parse(data[MyPlayerData.AvatarIndexKey].Value.ToString());
                                }

                                Common.ScreenLog.ForceLog("Getting opponent data");
                                getOpponentData(data, index, fbAvatar, avatarIndex, otherID);
                            }
                            else
                            {

                                Common.ScreenLog.ForceLog("ERROR 0");
                            }
                        }
                    }
                    else
                    {
                        Common.ScreenLog.ForceLog("ERROR 1");
                    }

                }, (error) =>
                {
                    Common.ScreenLog.ForceLog("Error 0: " + error.ErrorMessage);
                    Debug.Log("Get user data error: " + error.ErrorMessage);
                }, null);
            }
        }
    }




    public void CreatePrivateRoom()
    {
        GameManager.Instance.JoinedByID = false;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;


        string roomName = "";
        for (int i = 0; i < 8; i++)
        {
            roomName = roomName + UnityEngine.Random.Range(0, 10);
        }

        roomOptions.CustomRoomPropertiesForLobby = new String[] { "pc" };
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {
            { "pc", GameManager.Instance.payoutCoins}
         };
        Debug.Log("Private room name: " + roomName);
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
        roomOwner = true;
        GameManager.Instance.roomOwner = true;
        GameManager.Instance.currentPlayersCount = 1;
        GameManager.Instance.controlAvatars.updateRoomID(PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom called");
        roomOwner = false;
        GameManager.Instance.roomOwner = false;
        GameManager.Instance.resetAllData();
    }

    public int GetFirstFreeSlot()
    {
        int index = 0;
        for (int i = 0; i < GameManager.Instance.opponentsIDs.Count; i++)
        {
            if (GameManager.Instance.opponentsIDs[i] == null)
            {
                index = i;
                break;
            }
        }
        return index;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Common.ScreenLog.ForceLog("Failed to create room: " + message);
        CreatePrivateRoom();
        //loadSceneMenu();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Common.ScreenLog.ForceLog("Failed to join room: " + message);
        //JoinFailed(message);
        //CreatePrivateRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Common.ScreenLog.ForceLog("Failed to join random room: " + message);
        //JoinFailed(message);
        //CreatePrivateRoom();


        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomPropertiesForLobby = new String[] { "m", "v" };

        string BotMoves = generateBotMoves();

        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {
            { "m", GameManager.Instance.mode.ToString() +  GameManager.Instance.type.ToString() + GameManager.Instance.payoutCoins.ToString()},
            {"bt", BotMoves},
            {"fp", UnityEngine.Random.Range(0, GameManager.Instance.requiredPlayers)}
         };

        Debug.Log("Create Room: " + GameManager.Instance.mode.ToString() + GameManager.Instance.type.ToString() + GameManager.Instance.payoutCoins.ToString());
        roomOptions.MaxPlayers = (byte)GameManager.Instance.requiredPlayers;
        //roomOptions.IsVisible = true;

        StartCoroutine(TryToCreateGameAfterFailedToJoinRandom(roomOptions));
    }

    private static void JoinFailed(string message)
    {
        if (GameManager.Instance.type == MyGameType.Private)
        {
            if (GameManager.Instance.controlAvatars != null)
            {
                GameManager.Instance.controlAvatars.ShowJoinFailed(message);
            }
        }
        else
        {
            GameManager.Instance.facebookManager.startRandomGame();
        }
    }

    private void GetPlayerDataRequest(string playerID)
    {

    }

    public override void OnPlayerEnteredRoom(PhotonPlayer newPlayer)
    {
        CancelInvoke("StartGameWithBots");

        Debug.Log("New player joined " + newPlayer.NickName);
        Debug.Log("Players Count: " + GameManager.Instance.currentPlayersCount);



        if (PhotonNetwork.CurrentRoom.PlayerCount >= GameManager.Instance.requiredPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            GameManager.Instance.controlAvatars.startButtonPrivate.GetComponent<Button>().interactable = true;
        }
        else
        {
            GameManager.Instance.controlAvatars.startButtonPrivate.GetComponent<Button>().interactable = true;
        }

        int index = GetFirstFreeSlot();

        GameManager.Instance.opponentsIDs[index] = newPlayer.NickName;
        GetUserDataRequest getdatarequest = new GetUserDataRequest()
        {
            PlayFabId = newPlayer.NickName,
        };

        PlayFabClientAPI.GetUserData(getdatarequest, (result) =>
        {
            Dictionary<string, UserDataRecord> data = result.Data;

            if (data.ContainsKey("LoggedType"))
            {
                if (data["LoggedType"].Value.Equals("Facebook"))
                {
                    bool fbAvatar = true;
                    int avatarIndex = 0;
                    if (!data[MyPlayerData.AvatarIndexKey].Value.Equals("fb"))
                    {
                        fbAvatar = false;
                        avatarIndex = int.Parse(data[MyPlayerData.AvatarIndexKey].Value.ToString());
                    }
                    getOpponentData(data, index, fbAvatar, avatarIndex, newPlayer.NickName);
                }
                else
                {
                    if (data.ContainsKey("PlayerName"))
                    {
                        GameManager.Instance.opponentsNames[index] = data["PlayerName"].Value;
                        //GameManager.Instance.controlAvatars.PlayerJoined(index);
                        bool fbAvatar = true;
                        int avatarIndex = 0;
                        if (!data[MyPlayerData.AvatarIndexKey].Value.Equals("fb"))
                        {
                            fbAvatar = false;
                            avatarIndex = int.Parse(data[MyPlayerData.AvatarIndexKey].Value.ToString());
                        }
                        getOpponentData(data, index, fbAvatar, avatarIndex, newPlayer.NickName);
                    }
                    else
                    {
                        Common.ScreenLog.ForceLog("ERROR 2");
                    }
                }
            }
            else
            {
                Common.ScreenLog.ForceLog("ERROR 3");
            }

        }, (error) =>
        {
            Common.ScreenLog.ForceLog("Error 1: " + error.ErrorMessage);
            Debug.Log("Get user data error: " + error.ErrorMessage);
        }, null);




    }

    private void getOpponentData(Dictionary<string, UserDataRecord> data, int index, bool fbAvatar, int avatarIndex, string id)
    {
        if (data.ContainsKey("PlayerName"))
        {
            GameManager.Instance.opponentsNames[index] = data["PlayerName"].Value;
        }
        else
        {
            GameManager.Instance.opponentsNames[index] = "Guest857643";
        }

        if (data.ContainsKey("PlayerAvatarUrl") && fbAvatar)
        {
            StartCoroutine(loadImageOpponent(data["PlayerAvatarUrl"].Value, index, id));
        }
        else
        {
            Debug.Log("GET OPPONENT DATA: " + avatarIndex);

            Common.ScreenLog.ForceLog("Get opponent data");

            //Here we put the if statement
            if (GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().avatars[avatarIndex] != null)
            {
                GameManager.Instance.opponentsAvatars[index] = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().avatars[avatarIndex];
                //GameManager.Instance.opponentsAvatars[index] = null;
                GameManager.Instance.controlAvatars.PlayerJoined(index, id);
            }
            else
            {
                Common.ScreenLog.ForceLog("Something is wrong in avatar");
            }
        }

    }

    public IEnumerator loadImageOpponent(string url, int index, string id)
    {
        WWW www = new WWW(url);

        yield return www;

        GameManager.Instance.opponentsAvatars[index] = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 32);
        GameManager.Instance.controlAvatars.PlayerJoined(index, id);
    }

    public void OnUserSubscribed(string channel, string user)
    {
        throw new NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new NotImplementedException();
    }
}
