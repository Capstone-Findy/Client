using System;
using UnityEngine;

namespace Findy.Define
{
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
    [Serializable]
    public class StageProgressDto
    {
        public string country;
        public int stage;
        public float clearTime;
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
