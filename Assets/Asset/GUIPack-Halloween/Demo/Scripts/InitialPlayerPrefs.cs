// Copyright (C) 2017 ricimi. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at https://unity.com/legal/as-terms.

using UnityEngine;

namespace Ricimi
{
    // Utility class to force the music and sound effects to be enabled on first launch.
    public class InitialPlayerPrefs : MonoBehaviour
    {
        private void Awake()
        {
            if (!PlayerPrefs.HasKey("music_on"))
            {
                PlayerPrefs.SetInt("music_on", 1);
            }

            if (!PlayerPrefs.HasKey("sound_on"))
            {
                PlayerPrefs.SetInt("sound_on", 1);
            }
        }
    }
}