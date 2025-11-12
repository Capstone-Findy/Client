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
}
