using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Findy.Define
{
    //----- Login/SignUp -----//
    [Serializable]
    public class SignUpRequestDto
    {
        public string name;
        public string file;
        public string email;
        public string password;
    }
    [Serializable]
    public class LoginRequestDto
    {
        public string email;
        public string password;
        public bool rememberMe;
    }
    [Serializable]
    public class LoginResponseWrapper
    {
        public bool error;
        public string message;
        public AuthResponse data;
    }
    [Serializable]
    public class EmailValidationDto
    {
        public string email;
    }
    [Serializable]
    public class RefreshRequestDto
    {
        public string refreshToken;
    }

    [Serializable]
    public class AuthResponse
    {
        public string accessToken;
        public string refreshToken;
    }
    [Serializable]
    public class LogoutRequestDto
    {
        public int id;
    }

    //----- User Info -----//
    [Serializable]
    public class UserDataDto
    {
        public int id;
        public string name;
        public int money;
        public int heart;
        public int item1;
        public int item2;
        public int item3;
        public int item4;
    }
    [Serializable]
    public class GetUserInfoWrapper
    {
        public bool error;
        public string message;
        public UserDataDto data;
    }

    //----- Game Result -----//
    [Serializable]
    public class GameResultDto
    {
        public int gameId;
        public int remainTime;
        public int correct;
        public int item1;
        public int item2;
        public int item3;
        public int item4;
    }
    [Serializable]
    public class StageScoreDto
    {
        public int gameId;
        public int score;
    }
    [Serializable]
    public class GameScoreResponse
    {
        public bool error;
        public string message;
        public StageScoreDto[] data;
    }
    [Serializable]
    public class StageScoreListWrapper
    {
        public StageScoreDto[] scores;
    }
    //----- Item -----//
    [Serializable]
    public class ItemUpdateDto
    {
        public int item1;
        public int item2;
        public int item3;
        public int item4;
    }

    //----- AI Generator -----//
    public class AIDiffResponse
    {
        public string base_image;     
        public string diff_image;    
        public string answer_image; 
        public string mask_image;   
        
        public JObject coordinates;
    }
    public class CoordinateWrapper
    {
        public List<List<float>> canvas;
    }

    //----- Sound -----//
    public enum SoundType
    {
        BGM_Main1,
        BGM_Main2,
        BGM_InGame,
        SFX_Click,
        SFX_Correct,
        SFX_Wrong,
        SFX_AlreadyFound,
        SFX_Item,
        SFX_GameWin,
        SFX_GameOver,
        SFX_Purchase,
        SFX_PopupOpen
    }
}
