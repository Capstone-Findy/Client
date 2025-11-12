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

    [Serializable]
    public class UserItemsDto
    {
        public int hintCount;
        public int timeAddCount;
        public int overlapCount;
        public int gambleCount;
    }

    [Serializable]
    public class UpdateItemDto
    {
        public string itemType;
        public int count;
    }
}
