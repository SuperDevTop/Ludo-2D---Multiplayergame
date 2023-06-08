using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using AssemblyCSharp;
using System;
using UnityEngine;
using System.Collections;

public class MyPlayerData
{

    public static string TitleFirstLoginKey = "TitleFirstLogin";
    public static string TotalEarningsKey = "TotalEarnings";
    public static string GamesPlayedKey = "GamesPlayed";
    public static string TwoPlayerWinsKey = "TwoPlayerWins";
    public static string FourPlayerWinsKey = "FourPlayerWins";
    public static string PlayerName = "PlayerName";
    public static string CoinsKey = "Coins";
    public static string ChatsKey = "Chats";
    public static string EmojiKey = "Emoji";
    public static string AvatarIndexKey = "AvatarIndex";
    public static string FortuneWheelLastFreeKey = "FortuneWheelLastFreeTime";

    public Dictionary<string, UserDataRecord> data;

    public float GetCoins()
    {
        //if (this.data != null && this.data.ContainsKey(CoinsKey))
        //    return int.Parse(this.data[CoinsKey].Value);

        //fetch data from genie server 
        if (GenieAPIClass.Instance.IsLoggedIn)
            return Ludo.IN.GameManager.Instance.Genie_user_funds;
        else return 0;
    }

    public int GetTotalEarnings()
    {
        if (this.data.ContainsKey(TotalEarningsKey))
            return int.Parse(this.data[TotalEarningsKey].Value);
        else return 0;
    }

    public int GetTwoPlayerWins()
    {
        if (this.data.ContainsKey(TwoPlayerWinsKey))
            return int.Parse(this.data[TwoPlayerWinsKey].Value);
        else return 0;
    }

    public int GetFourPlayerWins()
    {
        if (this.data.ContainsKey(FourPlayerWinsKey))
            return int.Parse(this.data[FourPlayerWinsKey].Value);
        else return 0;
    }

    public int GetPlayedGamesCount()
    {
        if (this.data.ContainsKey(GamesPlayedKey))
            return int.Parse(this.data[GamesPlayedKey].Value);
        return 0;
    }

    public string GetAvatarIndex()
    {
        if (data.ContainsKey(AvatarIndexKey))
            return this.data[AvatarIndexKey].Value;
        else return "0";
    }

    public string GetChats()
    {
        if (this.data.ContainsKey(ChatsKey))
            return this.data[ChatsKey].Value;
        else return "";
    }

    public string GetEmoji()
    {
        if (this.data.ContainsKey(EmojiKey))
            return this.data[EmojiKey].Value;
        else return "";
    }

    public string GetPlayerName()
    {
        //if (this.data.ContainsKey(PlayerName))
        //    return this.data[PlayerName].Value;
        if (Ludo.IN.GameManager.Instance.nameMy != null)
            return Ludo.IN.GameManager.Instance.nameMy;
        else return "Error";
    }

    //public string GetLastFortuneTime()
    //{
    //    if (this.data.ContainsKey(FortuneWheelLastFreeKey))
    //    {
    //        return this.data[FortuneWheelLastFreeKey].Value;

    //    }
    //    else
    //    {
    //        string date = DateTime.Now.Ticks.ToString();
    //        Dictionary<string, string> data = new Dictionary<string, string>();
    //        data.Add(FortuneWheelLastFreeKey, date);
    //        UpdateUserData(data);
    //        return date;
    //    }
    //}



    public MyPlayerData() { }
    public MyPlayerData(Dictionary<string, UserDataRecord> data, bool myData)
    {
        this.data = data;


        if (myData)
        {
            if (GetAvatarIndex().Equals("fb"))
            {
                GameManager.Instance.avatarMy = GameManager.Instance.facebookAvatar;
            }
            else
            {
                GameManager.Instance.avatarMy = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().avatars[int.Parse(GetAvatarIndex())];
            }

            GameManager.Instance.nameMy = GetPlayerName();
        }

        Debug.Log("MY DATA LOADED");
    }


    bool isUpdatingData;

    public void UpdateUserData(Dictionary<string, string> data, GameObject popup = null, MonoBehaviour context = null)
    {
        if (isUpdatingData)
        {
            if (context != null)
                context.StartCoroutine(UpdateDataCoroutine(data, popup));
            return;
        }

        if (this.data != null)
            foreach (var item in data)
            {
                Debug.Log("SAVE: " + item.Key);
                if (this.data.ContainsKey(item.Key))
                {
                    Debug.Log("AA");
                    this.data[item.Key].Value = item.Value;

                }
            }

        UpdateUserDataRequest userDataRequest = new UpdateUserDataRequest()
        {
            Data = data,
            Permission = UserDataPermission.Public
        };

        isUpdatingData = true;

        if (popup) popup.SetActive(true);

        PlayFabClientAPI.UpdateUserData(userDataRequest, (result1) =>
        {
            isUpdatingData = false;
            if (popup) popup.SetActive(false);
            Debug.Log("Data updated successfull" + result1);

        }, (error1) =>
        {
            isUpdatingData = false;
            if (popup) popup.SetActive(false);
            Debug.Log("Data updated error " + error1.ErrorMessage);
        }, null);

    }

    IEnumerator UpdateDataCoroutine(Dictionary<string, string> data, GameObject popup)
    {
        yield return new WaitUntil(() => isUpdatingData == false);
        UpdateUserData(data, popup);
    }

    public static Dictionary<string, string> InitialUserData(bool fb)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add(TotalEarningsKey, "0");
        data.Add(ChatsKey, "");
        data.Add(EmojiKey, "");
        if (fb)
        {
            data.Add(CoinsKey, StaticStrings.initCoinsCountFacebook.ToString());
            data.Add(AvatarIndexKey, "fb");
        }
        else
        {
            data.Add(CoinsKey, StaticStrings.initCoinsCountGuest.ToString());
            data.Add(AvatarIndexKey, "0");
        }

        data.Add(GamesPlayedKey, "0");
        data.Add(TwoPlayerWinsKey, "0");
        data.Add(FourPlayerWinsKey, "0");

        data.Add(TitleFirstLoginKey, "1");
        data.Add(FortuneWheelLastFreeKey, DateTime.Now.Ticks.ToString());
        return data;
    }


}