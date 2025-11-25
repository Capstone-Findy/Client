using System;
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
    public class AuthResponse
    {
        public string token;
        public string refreshToken;
    }
    //----- User Info -----//
    [Serializable]
    public class UserDataDto
    {
        public string name;
        public int money;
        public int heart;
        public int item1;
        public int item2;
        public int item3;
        public int item4;
    }
    [Serializable]
    public class HeartUpdateDto
    {
        public int heart;
    }

    //----- Game Result -----//
    [Serializable]
    public class StageProgressDto
    {
        public string country;
        public int stage;
        public float clearTime;
    }
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
        SFX_GameOver
    }
}
