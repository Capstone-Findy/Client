using System.Collections;
using System.Collections.Generic;
using Findy.Define;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "AudioData")]
public class AudioData : ScriptableObject
{
    [System.Serializable]
    public class SoundData
    {
        public SoundType type;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    public List<SoundData> soundList = new List<SoundData>();
    
    public Dictionary<SoundType, AudioClip> GetAudioClip()
    {
        Dictionary<SoundType, AudioClip> dict = new Dictionary<SoundType, AudioClip>();
        foreach (var data in soundList)
        {
            if(!dict.ContainsKey(data.type))
            {
                dict.Add(data.type, data.clip);
            }
        }
        return dict;
    }
}
