using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
//using Photon.Pun;
using SimpleJSON;
//using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

using Ludo.IN;

public class GenieAPIClass : MonoBehaviour
{
    public static GenieAPIClass Instance;
    public bool IsLoggedIn = false;

    //remove default values before launch "Vinay.kumar@emptask.com", "12345678"
    public string LogIn_UserName = "", LogIn_Password = "", EmailID, SignUp_UserName, SignUp_Password;

    public GameObject LoadingImage, WrongID_Or_Pass;
    public GameObject LoginPanel, ErrorMessage;
    //private LoginInformation loginObject;
    public PhotonView PhotonController;

    public FacebookManager facebookManager;


    void SetLoginObject(Scene a, LoadSceneMode b)
    {
        if (SceneManager.GetActiveScene().name == "login")
        {
            //loginObject = GameObject.Find("Information").GetComponent<LoginInformation>();
        }
        else if (SceneManager.GetActiveScene().name == "MenuScene")
        {
            if (PhotonController == null)
            {
                PhotonController = gameObject.AddComponent<PhotonView>();
                PhotonController.ViewID = 99;
            }
            //PhotonController = GameObject.Find("PhotonObject").GetComponent<PhotonView>();
        }

        if (a.name == "find friends 8 ball")
        {
            FriendsCounter = 0;
        }

    }


    void Awake()
    {
        SceneManager.sceneLoaded += SetLoginObject;
        facebookManager = GameObject.Find("FacebookManager").GetComponent<FacebookManager>();


#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif

        DontDestroyOnLoad(this);

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (AutoLogin())
        {
            Debug.Log("Logging in");
        }
    }

    public bool AutoLogin()
    {
        if (PlayerPrefs.HasKey(Ludo.IN.GameManager.AutoLoginUsername) &&
            PlayerPrefs.HasKey(Ludo.IN.GameManager.AutoLoginPassword))
        {
            LoginUser(PlayerPrefs.GetString(Ludo.IN.GameManager.AutoLoginUsername),
                PlayerPrefs.GetString(Ludo.IN.GameManager.AutoLoginPassword));

            return true;
        }

        return false;
    }

    //public void ShowLoading()
    //{
    //    LoadingImage.SetActive(true);
    //}
    //public void HideLoading()
    //{
    //    LoadingImage.SetActive(false);
    //}

    IEnumerator RegisterUser()
    {
        yield return new WaitForSeconds(0f);
        //WWWForm form = new WWWForm();

        //form.AddField("username", LogIn_UserName);
        //form.AddField("email_id", EmailID);
        //form.AddField("password", LogIn_Password);


        //UnityWebRequest uwr = UnityWebRequest.Post(GinieAPILinks.PostOAuth, form);

        //yield return uwr.SendWebRequest();

        //if (uwr.isNetworkError || uwr.isHttpError)
        //{
        //    Debug.Log(uwr.error);
        //    Debug.Log("<color=red> Form upload error! = " + uwr.downloadHandler.text + "</color>");
        //}
        //else
        //{
        //    Debug.Log("Form upload complete!   =  " + uwr.downloadHandler.text);
        //    var N = JSON.Parse(uwr.downloadHandler.text);
        //    bool isUserExist = N["exists"];
        //}

    }

    /// <summary>
    /// Register new user 
    /// Send user to external browser to create new account
    /// </summary>
    public void OnRegisterNewUser()
    {
        Debug.Log("OnRegisterNewUser");
        Common.ScreenLog.ForceLog("OnRegisterNewUser");
        //LinkOpener.OpenLinkStatic("https://google.com");
        LinkOpener.OpenLinkStatic("https://geniegaming.com/register");
        //Application.OpenURL("https://geniegaming.com/register");
    }

    /// <summary>
    /// Retrieve password
    /// Send user to external browser to get new password
    /// </summary>
    public void OnForgetPassword()
    {
        Debug.Log("OnForgetPassword");
        Common.ScreenLog.ForceLog("OnForgetPassword");
        //LinkOpener.OpenLinkStatic("https://google.com");
        LinkOpener.OpenLinkStatic("https://geniegaming.com/password/reset");
        //Application.OpenURL("https://geniegaming.com/password/reset");
    }

    IEnumerator DisableObjectWithDelay(GameObject thing, float time)
    {
        yield return new WaitForSeconds(time);
        thing.SetActive(false);
    }

    /// <summary>
    /// login user with username and password
    /// </summary>
    public void OnLoginButtonClick()
    {
        facebookManager.showLoadingCanvas();
        LoadingImage.SetActive(true);
        LoginPanel.SetActive(false);
        //disable the login canvas
        StartCoroutine(LoginUser());
    }

    IEnumerator LoginUser()
    {
        WWWForm form = new WWWForm();
        form.AddField("grant_type", Ludo.IN.GameManager.Instance.Genie_grant_type);
        form.AddField("client_id", Ludo.IN.GameManager.Instance.Genie_client_id);
        form.AddField("client_secret", Ludo.IN.GameManager.Instance.Genie_client_secret);
        form.AddField("username", LogIn_UserName);
        form.AddField("password", LogIn_Password);

        UnityWebRequest uwr = UnityWebRequest.Post(GenieAPILinks.PostOAuth, form);

        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.Log(uwr.error);
            Debug.Log("<color=red> Form upload error! = " + uwr.downloadHandler.text + "</color>");
            StartCoroutine(SetLoginPanelActive(true));
            //play some error message

            //loginObject.ShowLoginErrorMssg("Username or Password Incorrect");
        }
        else
        {
            Debug.Log("Form upload complete!   =  " + uwr.downloadHandler.text);

            var N = JSON.Parse(uwr.downloadHandler.text);
            Ludo.IN.GameManager.Instance.Genie_access_token_Key = N["access_token"];

            PlayerPrefs.SetString(Ludo.IN.GameManager.Instance.Genie_Username_PlayerPref_Key, LogIn_UserName);
            PlayerPrefs.SetString(Ludo.IN.GameManager.Instance.Genie_Password_PlayerPref_Key, LogIn_Password);

            PlayerPrefs.SetString(Ludo.IN.GameManager.AutoLoginUsername, LogIn_UserName);
            PlayerPrefs.SetString(Ludo.IN.GameManager.AutoLoginPassword, LogIn_Password);
            PlayerPrefs.Save();

            StartCoroutine(GetUserDetails());
            facebookManager.initSession();
            GameManager.Instance.logged = true;

            InvokeRepeating("UpdateUserFundsFromServer", 5, 5);

            //StartCoroutine(LoadMenuScene());
            //Debug.Log("access_token   =  " + GameManager.Instance.Ginie_access_token_Key);
            //Debug.Log("LogIn_UserName   =  " + LogIn_UserName);
            //Debug.Log("LogIn_Password   =  " + LogIn_Password);
            //Debug.Log("Ginie_Username_PlayerPref   =  " + PlayerPrefs.GetString(GameManager.Instance.Ginie_Username_PlayerPref_Key));
            //Debug.Log("Ginie_Password_PlayerPref   =  " + PlayerPrefs.GetString(GameManager.Instance.Ginie_Password_PlayerPref_Key));
        }
    }

    IEnumerator SetLoginPanelActive(bool value)
    {
        yield return new WaitForSeconds(0.5f);
        LoginPanel.SetActive(value);
        ErrorMessage.SetActive(true);
        LoadingImage.SetActive(false);
        StartCoroutine(DisableObjectWithDelay(ErrorMessage, 1f));
    }

    /// <summary>
    /// login with parameters
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public void LoginUser(string username, string password)
    {
        if (username != null) LogIn_UserName = username;
        if (password != null) LogIn_Password = password;

        OnLoginButtonClick();
    }

    //Load Menu Scene
    IEnumerator LoadMenuScene()
    {

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("MenuScene");
    }

    public void UpdateUserFundsFromServer()
    {
        StartCoroutine(UpdateFundsFromServer());
    }


    IEnumerator UpdateFundsFromServer()
    {
        Dictionary<string, string> headersWithBearerToken;
        headersWithBearerToken = new Dictionary<string, string> {
            { "Content-Type", "application/json" }, { "Authorization", "Bearer " + Ludo.IN.GameManager.Instance.Genie_access_token_Key}
        };

        //Debug.Log(" GameManager.Instance.Ginie_access_token_Key : " + Ludo.IN.GameManager.Instance.Genie_access_token_Key);

        WWW WwRequest = new WWW(GenieAPILinks.GetUsersDetail, null, headersWithBearerToken);

        yield return WwRequest;

        if (WwRequest.error == null)
        {
            var N = JSON.Parse(WwRequest.text);
            //GameManager.Instance.Genie_user_id = N["data"]["id"];
            //GameManager.Instance.Genie_username = N["data"]["username"];
            //GameManager.Instance.Genie_user_profile_image = N["data"]["profile_image"];
            Ludo.IN.GameManager.Instance.Genie_user_funds = N["data"]["available_balance"];
            //GameManager.Instance.Genie_user_banned = N["data"]["banned"];
            //GameManager.Instance.Genie_user_banned_reason = N["data"]["ban_reason"];
            //GameManager.Instance.Genie_games_won = N["data"]["games_won"];
            //GameManager.Instance.Genie_games_played = N["data"]["games_played"];
            //GameManager.Instance.Genie_win_percentage = N["data"]["win_percentage"];
            //GameManager.Instance.Genie_win_streak = N["data"]["win_streak"];
            //GameManager.Instance.Genie_country = N["data"]["country"];
            //yield return new WaitForSeconds(1f);

            //Load Scene after getting the user details 
            //var bigBrother = BigBrother.Instance;
            //bigBrother.Login();
            //bigBrother.ContinueAfterLogin();
            //if (!string.IsNullOrEmpty(GameManager.Instance.Genie_user_profile_image))
            //{
            //    GameManager.Instance.HasProfileImageLink = true;
            //    GetMyProfilePictureFromAPIServer();
            //}
        }
    }


    IEnumerator GetUserDetails()
    {
        Dictionary<string, string> headersWithBearerToken;
        headersWithBearerToken = new Dictionary<string, string> {
            { "Content-Type", "application/json" }, { "Authorization", "Bearer " + Ludo.IN.GameManager.Instance.Genie_access_token_Key}
        };

        //Debug.Log(" GameManager.Instance.Ginie_access_token_Key : " + Ludo.IN.GameManager.Instance.Genie_access_token_Key);

        WWW WwRequest = new WWW(GenieAPILinks.GetUsersDetail, null, headersWithBearerToken);

        yield return WwRequest;

        if (WwRequest.error == null)
        {
            var N = JSON.Parse(WwRequest.text);
            Ludo.IN.GameManager.Instance.Genie_user_id = N["data"]["id"];
            Ludo.IN.GameManager.Instance.Genie_username = N["data"]["username"];
            Ludo.IN.GameManager.Instance.Genie_account_number = N["data"]["account_number"];
            Ludo.IN.GameManager.Instance.Genie_user_profile_image = N["data"]["profile_image"];
            Ludo.IN.GameManager.Instance.Genie_user_funds = N["data"]["available_balance"];
            Ludo.IN.GameManager.Instance.Genie_user_banned = N["data"]["banned"];
            Ludo.IN.GameManager.Instance.Genie_user_banned_reason = N["data"]["ban_reason"];
            Ludo.IN.GameManager.Instance.Genie_games_won = N["data"]["games_won"];
            Ludo.IN.GameManager.Instance.Genie_games_played = N["data"]["games_played"];
            Ludo.IN.GameManager.Instance.Genie_win_percentage = N["data"]["win_percentage"];
            Ludo.IN.GameManager.Instance.Genie_win_streak = N["data"]["win_streak"];
            Ludo.IN.GameManager.Instance.Genie_country = N["data"]["country"];
            Ludo.IN.GameManager.Instance.Genie_user_county_image_link = N["data"]["county_image"];
            Ludo.IN.GameManager.Instance.nameMy = Ludo.IN.GameManager.Instance.Genie_username;

            if (!string.IsNullOrEmpty(Ludo.IN.GameManager.Instance.Genie_user_county_image_link))
            {
                Debug.Log("GG = " + Ludo.IN.GameManager.Instance.Genie_user_county_image_link);
                GetCountryPictureFromAPIServer();
            }

            Debug.Log("Form upload complete login!   =  " + WwRequest.text);

            yield return new WaitForSeconds(1f);
            //Load Scene after getting the user details 
            //var bigBrother = BigBrother.Instance;
            //bigBrother.Login();
            //bigBrother.ContinueAfterLogin();
        }
        else
        {
            Debug.LogError("Login: " + WwRequest.error);
        }
    }

    void OnGUI()
    {
        //if (GUI.Button(new Rect(10, 10, 150, 100), "LogOutFB"))
        //{
        //    FBLogin.Instance.LogoutFacebook();
        //    print("You clicked LogOutFB button!");
        //}
    }

    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(10, 10, 150, 100), "CreateGame"))
    //    {
    //        //GetMyProfilePictureFromAPIServer();
    //        StartCoroutine(CreateGame("", ""));
    //        print("You clicked the button!");
    //    }

    //    if (GUI.Button(new Rect(200, 10, 150, 100), "FinishGame"))
    //    {
    //        //GetMyProfilePictureFromAPIServer();
    //        StartCoroutine(FinishGame(""));
    //        print("You clicked the button!");
    //    }
    //}



    public static byte[] StringToByteArray(String Source)
    {

        char[] CSource = Source.ToCharArray();
        byte[] Result = new byte[CSource.Length];

        for (Int32 i = 0; i < CSource.Length; i++)
            Result[i] = Convert.ToByte(CSource[i]);

        return Result;

    }

    public static String ByteArrayToString(byte[] Source)
    {
        char[] CSource = new char[Source.Length];
        for (Int32 i = 0; i < Source.Length; i++)
            CSource[i] = Convert.ToChar(Source[i]);
        return new String(CSource);
    }

    /// <summary>
    /// Create a room instance on genie gaming when player are joined and ready
    /// </summary>
    /// <param name="Opponent_id"></param>
    /// <param name="Room_id"></param>
    public void CreateTheGame(string Opponent_id, int Room_id)
    {

        StartCoroutine(CreateGame(Opponent_id, "" + Room_id, "", ""));
    }

    public void CreateFourPlayerGame(string Opponent_id, string Opponent_id2, string Opponent_id3, int Room_id)
    {
        StartCoroutine(CreateGame(Opponent_id, "" + Room_id, Opponent_id2, Opponent_id3));
    }
    //call it when player joins a room in poton
    IEnumerator CreateGame(string Opponent_id, string Room_id, string Opponent_id2, string Opponent_id3)
    {
        Dictionary<string, string> headersWithBearerToken;
        headersWithBearerToken = new Dictionary<string, string> {
            { "Content-Type", "application/json" }, { "Authorization", "Bearer " + Ludo.IN.GameManager.Instance.Genie_access_token_Key}
        };

        CreateGameJsonGetSet GameInstance = new CreateGameJsonGetSet();

        Debug.Log("Opponent_id    -=-=-=-=-=-=-   " + Opponent_id);
        Debug.Log("Room_id   -=-=-=-=-=-=- " + Room_id);
        Debug.Log("Toke:- " + headersWithBearerToken);
        if (GameManager.Instance.type == MyGameType.TwoPlayer)
        {
            GameInstance.players[0] = Ludo.IN.GameManager.Instance.Genie_user_id;
            GameInstance.players[1] = Opponent_id;   //have to get this when matchmaking in photon 
            GameInstance.room_id = Room_id;      //get this from room selection scene bronze, silver etc...
        }
        else
        {
            if (GameManager.Instance.type == MyGameType.FourPlayer)
            {
                GameInstance.players = new string[4];
                GameInstance.players[0] = Ludo.IN.GameManager.Instance.Genie_user_id;
                GameInstance.players[1] = Opponent_id;   //have to get this when matchmaking in photon 
                GameInstance.players[2] = Opponent_id2;
                GameInstance.players[3] = Opponent_id3;
                GameInstance.room_id = Room_id;      //get this from room selection scene bronze, silver etc...                
            }
        }

        string playerToJason = JsonUtility.ToJson(GameInstance, true);
        Debug.Log("" + playerToJason);
        byte[] bodyRaw = StringToByteArray(playerToJason);
        Debug.Log("" + ByteArrayToString(bodyRaw));

        WWW uwr = new WWW(GenieAPILinks.PostCreateGameInstance, bodyRaw, headersWithBearerToken);
        Debug.Log("UWR:- " + uwr);
        yield return uwr;


        if (uwr.error == null)
        {
            Debug.Log("upload complete!   =  " + uwr.text);

            var N = JSON.Parse(uwr.text);
            Ludo.IN.GameManager.Instance.Genie_Game_Instance = N["data"]["id"];
            Ludo.IN.GameManager.Instance.Genie_Curr_room_fee = N["data"]["room"]["prize"];
            Ludo.IN.GameManager.Instance.Genie_isGame_inProgress = true;
            SendGameInstance();
            Debug.Log("Ginie_Game_ID   =  " + Ludo.IN.GameManager.Instance.Genie_Game_Instance);
            Debug.Log("entry_fee   =  " + Ludo.IN.GameManager.Instance.Genie_Curr_room_fee);
            //Matchmaker.Instance.OpenGameScene();
            //OpenGameScene();
            Common.ScreenLog.ForceLog("Create game success: " + uwr.text);
        }
        else
        {
            Ludo.IN.GameManager.Instance.Genie_isGame_inProgress = false;
            Debug.Log(uwr.error);
            Debug.Log("<color=red> Form upload error! = " + uwr.text + "</color>");

            Common.ScreenLog.ForceLog("Create game error: " + uwr.error + ", text: " + uwr.text);

            if (SceneManager.GetActiveScene().name == "matchmaking")
            {
                //Matchmaker.Instance.CancelMatchmaking();
            }
            else
            {
                CancelMatchmaking();
            }
        }
    }

    void CancelMatchmaking()
    {
        PhotonNetwork.Disconnect();
        //BigBrother.Instance.ShowScreen(ScreenType.Lobby);
    }


    //-=-==-=   For Ingame Assets Changes  like Cue and Avatar   -=-=-=-=-=-=-=-=-=-=-=-=-=


    public void SendMyCueAndAvatarID()
    {
        object[] objectArray = new object[2];

        //objectArray[0] = BigBrother.Instance.AvatarId;
        //objectArray[1] = BigBrother.Instance.CueId;

        Debug.Log("avatarId  =  " + objectArray[0]);
        Debug.Log("CueId  =  " + objectArray[1]);

        //PhotonController.RPC("SendCueAndAvatarIDs", RpcTarget.OthersBuffered, objectArray);
    }

    [PunRPC]
    void SendCueAndAvatarIDs(object[] objectArray)
    {
        GotOpponetCueAndAvatarID(objectArray);
    }

    public void GotOpponetCueAndAvatarID(object[] objectArray)
    {
        Debug.Log("avatarId  =  " + objectArray[0]);
        Debug.Log("CueId  =  " + objectArray[1]);

        Ludo.IN.GameManager.Instance.Genie_Oppo_Avid = (string)objectArray[0];
        Ludo.IN.GameManager.Instance.Genie_Oppo_CueId = (string)objectArray[1];

        //GameSceneAssetsUpdater.Instance.UpdateOppoAvatar();
        //GameSceneAssetsUpdater.Instance.UpdateOppoCue();
    }

    //-=-==-=-=-=-=-=  For Creating Game and stuff      -=-=-=-=-=-=-=-=-=

    public void SendMyUserIDAndName()
    {
        object[] objectArray = new object[1];

        string user_id = Ludo.IN.GameManager.Instance.Genie_user_id;
        //objectArray[1] = BigBrother.Instance.AvatarId;
        //objectArray[2] = BigBrother.Instance.CueId;

        Debug.Log("UserID  =  " + user_id);
        //Debug.Log("avatarId  =  " + objectArray[1]);
        //Debug.Log("CueId  =  " + objectArray[2]);

        //PhotonController.RPC("SendUserID", RpcTarget.OthersBuffered, objectArray);
        PhotonController.RPC("SendUserID", RpcTarget.OthersBuffered, user_id);
    }

    public void SendGameInstance()
    {
        object[] objectArray = new object[2];

        objectArray[0] = Ludo.IN.GameManager.Instance.Genie_Game_Instance;
        objectArray[1] = Ludo.IN.GameManager.Instance.Genie_Game_Instance;
        Debug.Log("<color=blue>Game_Instance  =  </color>" + objectArray[0]);

        //PhotonController.RPC("ReceivedGameInstance", RpcTarget.OthersBuffered, objectArray);
        PhotonController.RPC("ReceivedGameInstance", RpcTarget.OthersBuffered, objectArray);
    }

    [PunRPC]
    void SendUserID(string user_id)
    {
        Common.ScreenLog.ForceLog("I am in SendUserID method");
        GotOpponetUserID(user_id);
    }

    [PunRPC]
    void ReceivedGameInstance(object[] objectArray)
    {
        Common.ScreenLog.ForceLog("Received Game Instance");
        GotGameInstance(objectArray);
    }


    public string[] opp_Id = new string[3];
    public int count = 1;
    public void GotOpponetUserID(string user_id)
    {
        Debug.Log("Hello");
        //Debug.Log("UserID  =  " + objectArray[0]);
        //Debug.Log("avatarId  =  " + objectArray[1]);
        //Debug.Log("CueId  =  " + objectArray[2]);
        //Ludo.IN.GameManager.Instance.Genie_Oppo_Avid = (string)objectArray[1];
        //Ludo.IN.GameManager.Instance.Genie_Oppo_CueId = (string)objectArray[2];
        if (GameManager.Instance.type == MyGameType.TwoPlayer)
        {
            Ludo.IN.GameManager.Instance.Genie_Oppo_id = user_id;
        }
        else
        {
            if (GameManager.Instance.type == MyGameType.FourPlayer)
            {
                opp_Id[count - 1] = user_id;
                if (count != 3)
                {
                    count++;
                    return;
                }
                else
                {
                    count = 1;
                }
            }
        }

        Debug.Log("GameManager.Instance.Genie_isGame_inProgress  =  " + Ludo.IN.GameManager.Instance.Genie_isGame_inProgress);
        if (GameManager.Instance.roomOwner && !Ludo.IN.GameManager.Instance.Genie_isGame_inProgress)
        {
            Debug.Log("Creating a game from API ");
            Ludo.IN.GameManager.Instance.Genie_isGame_inProgress = true;
            if (GameManager.Instance.type == MyGameType.TwoPlayer)
            {
                GenieAPIClass.Instance.CreateTheGame(Ludo.IN.GameManager.Instance.Genie_Oppo_id, Ludo.IN.GameManager.Instance.Genie_Curr_roomId);
            }
            else
            {
                if (GameManager.Instance.type == MyGameType.FourPlayer)
                {
                    GenieAPIClass.Instance.CreateFourPlayerGame(opp_Id[0], opp_Id[1], opp_Id[2], Ludo.IN.GameManager.Instance.Genie_Curr_roomId);
                }
            }
        }

        //OpponentSearchAnimatonController.OpponentFound = true;
        if (GameManager.Instance.type == MyGameType.TwoPlayer)
        {
            StartCoroutine(GetUserdata(Ludo.IN.GameManager.Instance.Genie_Oppo_id));
        }
        else
        {
            if (GameManager.Instance.type == MyGameType.FourPlayer)
            {
                //We have to put code here for four players
            }
        }
    }


    public void GotGameInstance(object[] objectArray)
    {
        Common.ScreenLog.ForceLog("Got Game Instance");
        //if (!GameManager.Instance.roomOwner)
        //{
        Ludo.IN.GameManager.Instance.Genie_Game_Instance = (string)objectArray[0];
        Debug.Log(objectArray[0] + " <color=red> Game_Instance  = </color> " + Ludo.IN.GameManager.Instance.Genie_Game_Instance);
        //}
    }

    //Get Friends little Detail for the list 
    public void GetFriendsDetails()
    {
        Ludo.IN.GameManager.Instance.Genie_user_friends_name = new string[Ludo.IN.GameManager.Instance.Genie_user_friends_count];
        foreach (string id in Ludo.IN.GameManager.Instance.Genie_user_friends_user_id)
        {
            StartCoroutine(GetUserdata(id, true));
        }
    }

    int FriendsCounter = 0;
    // Show Oppnent User Profile From Here
    IEnumerator GetUserdata(string userId, bool isfriend = false)
    {
        Dictionary<string, string> headersWithBearerToken;
        headersWithBearerToken = new Dictionary<string, string> {
            { "Content-Type", "application/json" }, { "Authorization", "Bearer " + Ludo.IN.GameManager.Instance.Genie_access_token_Key}
        };

        //Debug.Log(" GameManager.Instance.Ginie_access_token_Key : " + Ludo.IN.GameManager.Instance.Genie_access_token_Key);

        WWW WwRequest = new WWW(GenieAPILinks.GetOpponentDetails + userId, null, headersWithBearerToken);

        yield return WwRequest;

        if (WwRequest.error == null)
        {
            var N = JSON.Parse(WwRequest.text);
            if (!isfriend)
            {
                Ludo.IN.GameManager.Instance.Genie_Oppo_username = N["data"]["username"];
                Ludo.IN.GameManager.Instance.Genie_Oppo_games_won = N["data"]["games_won"];
                Ludo.IN.GameManager.Instance.Genie_Oppo_games_played = N["data"]["games_played"];
                Ludo.IN.GameManager.Instance.Genie_Oppo_win_percentage = N["data"]["win_percentage"];
                Ludo.IN.GameManager.Instance.Genie_Oppo_win_streak = N["data"]["win_streak"];
                Ludo.IN.GameManager.Instance.Genie_Oppo_country = N["data"]["country"];
                Ludo.IN.GameManager.Instance.Genie_Oppo_county_image_link = N["data"]["county_image"];
                Ludo.IN.GameManager.Instance.nameOpponent = Ludo.IN.GameManager.Instance.Genie_Oppo_username;
            }
            else if (isfriend)
            {
                //Debug.Log("  username = " + N["data"]["username"]);
                //Debug.Log(" FriendsCounter =  " + FriendsCounter);
                //Debug.Log("   GameManager.Instance.Genie_user_friends_name[FriendsCounter] = " + GameManager.Instance.Genie_user_friends_name[FriendsCounter]);
                Ludo.IN.GameManager.Instance.Genie_user_friends_name[FriendsCounter] = N["data"]["username"];
                FriendsCounter++;
                //GameManager.Instance.Genie_user_friends_avatar_id[FriendsCounter] = N["data"]["username"];
                if (FriendsCounter == Ludo.IN.GameManager.Instance.Genie_user_friends_count)
                {
                    //FriendListHandler.Instance.LoadingScreen.SetActive(false);
                    //FriendListHandler.Instance.CreateFriendList();
                    FriendsCounter = 0;
                }
            }

            Common.ScreenLog.ForceLog("Get user details success: " + WwRequest.text);
        }
        else
        {
            Common.ScreenLog.ForceLog("Get user details error: " + WwRequest.error);
        }
    }


    //public void OpenGameScene()
    //{
    //    Debug.Log("ShowGameScreenWithDelay   8 Ball Pool   ");
    //    BigBrother.Instance.ShowScreen(ScreenType.Game8Ball);
    //}



    //-=-==-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=



    public string convertJSON_To_JSONArray(string PlayerToJason)
    {
        string[] array = PlayerToJason.Split('[');
        array = array[1].Split(']');
        array[0] = "[" + array[0] + "]";
        string returnValueInArray = array[0];
        return returnValueInArray;
    }



    IEnumerator GetSingleGame()
    {
        Dictionary<string, string> headersWithBearerToken;
        headersWithBearerToken = new Dictionary<string, string> {
            { "Content-Type", "application/json" }, { "Authorization", "Bearer " + Ludo.IN.GameManager.Instance.Genie_access_token_Key}
        };
        //Debug.Log(" GameManager.Instance.Ginie_access_token_Key : " + Ludo.IN.GameManager.Instance.Genie_access_token_Key);

        WWW WwRequest = new WWW(GenieAPILinks.GetSingleGame, null, headersWithBearerToken);

        yield return WwRequest;

        if (WwRequest.error == null)
        {
            var N = JSON.Parse(WwRequest.text);
            //GameManager.Instance.Ginie_user_id = N["data"]["id"];
            //GameManager.Instance.Ginie_username = N["data"]["username"];
            //GameManager.Instance.Ginie_user_profile_image = N["data"]["profile_image"];
            //GameManager.Instance.Ginie_user_banned = N["data"]["banned"];
            //GameManager.Instance.Ginie_user_banned_reason = N["data"]["ban_reason"];
            Debug.Log("Get complete!   =  " + WwRequest.text);
            yield return new WaitForSeconds(1f);

            //Load Scene after getting the user details 
            //var bigBrother = BigBrother.Instance;
            //bigBrother.Login();
            //bigBrother.ContinueAfterLogin();

        }
    }

    /// <summary>
    /// Called when game ends with winner id to declare winner to the genie gaming website
    /// </summary>
    /// <param name="Winner_id"></param>
    /// <returns></returns>
    public void FinishTheGame(string Winner_id)
    {
        StartCoroutine(FinishGame(Winner_id));
        Debug.Log("Winner_id = " + Winner_id);
    }


    IEnumerator FinishGame(string Winner_id)
    {
        Dictionary<string, string> headersWithBearerToken;
        headersWithBearerToken = new Dictionary<string, string> {
            { "Content-Type", "application/json" }, { "Authorization", "Bearer " + Ludo.IN.GameManager.Instance.Genie_access_token_Key}
        };

        EndGameJsonGetSet GameInstance = new EndGameJsonGetSet();

        GameInstance.action = "end";
        GameInstance.winner = Winner_id;      //get Winner Player ID

        string playerToJason = JsonUtility.ToJson(GameInstance, true);
        Debug.Log("" + playerToJason);
        byte[] bodyRaw = StringToByteArray(playerToJason);
        Debug.Log("" + ByteArrayToString(bodyRaw));
        Debug.Log(" " + Ludo.IN.GameManager.Instance.Genie_Game_Instance);
        WWW uwr = new WWW(GenieAPILinks.PUTEndGame + "" + Ludo.IN.GameManager.Instance.Genie_Game_Instance, bodyRaw, headersWithBearerToken);
        yield return uwr;
        if (uwr.error == null)
        {
            Debug.Log("upload complete!   =  " + uwr.text);

            var N = JSON.Parse(uwr.text);
            //Ludo.IN.GameManager.Instance.Ginie_Game_ID = N["data"]["id"];
            Ludo.IN.GameManager.Instance.Genie_isGame_inProgress = false;
            Debug.Log("status =  " + N["data"]["state"]);
        }
        else
        {

            Debug.Log(uwr.error);
            Debug.Log("<color=red> Form upload error! = " + uwr.text + "</color>");
        }

    }

    public void GetCountryPictureFromAPIServer()
    {
        StartCoroutine(loadImageMy(Ludo.IN.GameManager.Instance.Genie_user_county_image_link));
    }

    public IEnumerator loadImageMy(string url)
    {
        // Load avatar image

        // Start a download of the given URL
        WWW www = new WWW(url);

        // Wait for download to complete
        yield return www;

        Ludo.IN.GameManager.Instance.Genie_user_county_sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 32);

    }

    /// <summary>
    /// Sets the username of the player, and save it in the PlayerPrefs for future sessions.
    /// </summary>
    /// <param name="value">The name of the Player</param>
    public void SetUsername(string value)
    {
        // #Important
        if (string.IsNullOrEmpty(value))
        {
            //Debug.LogError("User Name is null or empty");
            return;
        }
        LogIn_UserName = value;

        PlayerPrefs.SetString(Ludo.IN.GameManager.Instance.Genie_Username_PlayerPref_Key, value);
    }

    /// <summary>
    /// Sets the password of the player, and save it in the PlayerPrefs for future sessions.
    /// </summary>
    /// <param name="value">The name of the Player</param>
    public void SetPassword(string value)
    {
        // #Important
        if (string.IsNullOrEmpty(value))
        {
            //Debug.LogError("Player Name is null or empty");
            return;
        }
        LogIn_Password = value;

        PlayerPrefs.SetString(Ludo.IN.GameManager.Instance.Genie_Password_PlayerPref_Key, value);
    }


}
