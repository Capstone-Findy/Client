// ----- C#
using System;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;
// using DG.Tweening;

public class SoundManager
{
    // --------------------------------------------------
    // Variables
    // --------------------------------------------------
    // ----- Private
    private AudioSource[] _audioSources = new AudioSource[(int)Define.ESoundType.MAX];
    private Dictionary<string, AudioClip> _audioClips = new();
    private GameObject _root = null;
    
    // --------------------------------------------------
    // Functions - Constructor & Destructor & Event
    // --------------------------------------------------
    public SoundManager()
    {
        AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChanged;
    }
    
    ~SoundManager()
    {
        AudioSettings.OnAudioConfigurationChanged -= OnAudioConfigurationChanged;
    }

    private void OnAudioConfigurationChanged(bool deviceWasChanged)
    {
        if (_audioSources.Length == 0)
            return;
        
        var bgmAudioSource = _audioSources[(int)Define.ESoundType.BGM];
        
        if (bgmAudioSource == null || bgmAudioSource.clip == null)
            return;

        var currentBGMClip = bgmAudioSource.clip;
        
        bgmAudioSource.volume = 0.5f; // TODO : Set Saved BGM Volume
        bgmAudioSource.clip = currentBGMClip;
        bgmAudioSource.Play();
        Debug.Log($"[SoundManager.OnAudioConfigurationChanged] Sound Environment Changed. BGM {currentBGMClip.name} Replayed");
    }
    
    // --------------------------------------------------
    // Functions - Nomal
    // --------------------------------------------------
    // ----- Public
    public void Init(Action doneCallback = null)
    {
        _root = GameObject.Find("@Sound");
        if (_root == null)
        {
            _root = new GameObject("@Sound");
            UnityEngine.Object.DontDestroyOnLoad(_root);
            
            var soundNames = Enum.GetNames(typeof(Define.ESoundType));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                if (i == (int)Define.ESoundType.MAX)
                    break;
                
                var go = new GameObject(soundNames[i]);
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = _root.transform;
            }
            
            _audioSources[(int)Define.ESoundType.BGM].loop = true;
        }
        
        doneCallback?.Invoke();
    }
    
    public void Play(string path, Define.ESoundType type = Define.ESoundType.SFX, float pitch = 1.0f, float volume = 1.0f)
    {
        var audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch, volume);
    }
    
    public void Play(AudioClip audioClip, Define.ESoundType type = Define.ESoundType.SFX, float pitch = 1.0f, float volume = 1.0f)
    {
        if (audioClip == null)
            return;
        
        var audioSource = _audioSources[(int)type];
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        
        if (type == Define.ESoundType.BGM)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            audioSource.PlayOneShot(audioClip);
        }
    }
    
    AudioClip GetOrAddAudioClip(string path, Define.ESoundType type = Define.ESoundType.SFX)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";

        AudioClip audioClip = null;

        if (type == Define.ESoundType.BGM)
        {
            audioClip = Managers.Resource.Load<AudioClip>(path);
        }
        else
        {
            if (_audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
                _audioClips.Add(path, audioClip);
            }
        }

        if (audioClip == null)
        {
            Debug.Log($"[SoundManager.Sound] AudioClip Missing! {path}");
        }

        return audioClip;
    }
    
    public void Clear()
    {
        foreach (var audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }
    
    public void DelayedPlay(string path, float delay, Define.ESoundType type = Define.ESoundType.SFX, float pitch = 1.0f)
    {
        // DOVirtual.DelayedCall(delay, () => Play(path, type, pitch));
    }
}
