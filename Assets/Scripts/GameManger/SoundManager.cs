using System.Collections;
using System.Collections.Generic;
using Findy.Define;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Data Asset")]
    [SerializeField] private AudioData audioData;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    private Dictionary<SoundType, AudioClip> clipDict;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
    }
    private void Initialize()
    {
        if(audioData != null)
        {
            clipDict = audioData.GetAudioClip();
        }
    }
    public void PlaySFX(SoundType type)
    {
        if(clipDict.ContainsKey(type))
        {
            sfxSource.PlayOneShot(clipDict[type]);
        }
    }
    public void PlayBGM(SoundType type)
    {
        if(clipDict.ContainsKey(type))
        {
            if(bgmSource.clip == clipDict[type] && bgmSource.isPlaying) return;
            bgmSource.clip = clipDict[type];
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }
    public void StopBGM()
    {
        bgmSource.Stop();
    }
}
