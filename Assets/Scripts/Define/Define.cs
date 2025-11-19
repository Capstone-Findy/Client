using System;
using UnityEngine;

namespace Findy.Define
{
    [Serializable]
    public class AuthResponse
    {
        public string token;
        public long userId;
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
