using System;
using System.Collections;
using System.Collections.Generic;
using Findy.Define;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

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

    void Start()
    {
        float bgmVol = PlayerPrefs.GetFloat("BGM_Volume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFX_Volume", 1f);

        SetBGMVolume(bgmVol);
        SetSFXVolume(sfxVol);

        bool isMute = PlayerPrefs.GetInt("MasterMute", 0) == 1;
        SetMasterMute(isMute);
    }
    private void Initialize()
    {
        if(audioData != null)
        {
            clipDict = audioData.GetAudioClip();
        }
    }

    public void SetBGMVolume(float sliderValue)
    {
        float volume = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;
        audioMixer.SetFloat("BGM", volume);

        PlayerPrefs.SetFloat("BGM_Volume", sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        float volume = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;
        audioMixer.SetFloat("SFX", volume);

        PlayerPrefs.SetFloat("SFX_Volume", sliderValue);
    }

    public void SetMasterMute(bool isMute)
    {
        float volume = isMute ? -80f : 0f;
        audioMixer.SetFloat("Master", volume);

        PlayerPrefs.SetInt("MasterMute", isMute ? 1: 0);
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
