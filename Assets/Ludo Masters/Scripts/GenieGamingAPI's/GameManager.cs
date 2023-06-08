using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ludo.IN
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        //for server handshake
        public string Genie_grant_type = "password";
        public string Genie_client_id = "3";
        public string Genie_client_secret = "US1JwtJgosvJ6kiIy0nO4KPrfiOgKlzjVhueUgHp";

        //user oAuth token
        public string Genie_access_token_Key;

        //user local data
        public string Genie_Username_PlayerPref_Key = "LogIn_UserName";
        public string Genie_Password_PlayerPref_Key = "LogIn_Password";

        //local auto login
        public static string AutoLoginUsername = "AutoLogin_UserName";
        public static string AutoLoginPassword = "AutoLogin_Password";

        //user info
        public string Genie_user_id;
        public string Genie_username;
        public string Genie_account_number;
        public string Genie_user_profile_image;
        public float Genie_user_funds;
        public string Genie_user_banned;
        public string Genie_user_banned_reason;
        public string Genie_games_won;
        public string Genie_games_played;
        public string Genie_win_percentage;
        public string Genie_win_streak;
        public string Genie_country;
        public string Genie_user_county_image_link;
        public Sprite Genie_user_county_sprite;
        public string nameMy;
        
        //room data
        public string Genie_Game_Instance;
        public string Genie_Curr_room_fee;
        public bool Genie_isGame_inProgress;        
        
        public bool roomOwner;
        public int Genie_Curr_roomId;

        //friends data
        public string[] Genie_user_friends_name;
        public string[] Genie_user_friends_user_id;
        public int Genie_user_friends_count;

        //opponent data
        public string Genie_Oppo_Avid;
        public string Genie_Oppo_CueId;
        public string Genie_Oppo_id;
        public string Genie_Oppo_username;
        public string Genie_Oppo_games_won;
        public string Genie_Oppo_games_played;
        public string Genie_Oppo_win_percentage;
        public string Genie_Oppo_win_streak;
        public string Genie_Oppo_country;
        public string Genie_Oppo_county_image_link;
        public string nameOpponent;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }
    }
}
