using System;
using System.Collections;
using UnityEngine;

public class HapticManager
{
    // --------------------------------------------------
    // Variables
    // --------------------------------------------------
    // ----- Const
    private const string VIBRATE = "vibrate";
    private const float DEFAULT_INTERVAL = 0.1f;
    
    // ----- Private
    private AndroidJavaObject _vibrator;
    private bool _isVibrating = false;
    private WaitForSeconds _waitInterval = new (DEFAULT_INTERVAL);
    
    // ----- Coroutine
    private Coroutine _vibrationCoroutine = null;

    // --------------------------------------------------
    // Functions - Nomal
    // --------------------------------------------------
    // ----- Public
    public void Vibrate(Define.EHapticType type = Define.EHapticType.Medium, int count = 1, float interval = DEFAULT_INTERVAL)
    {
        // TODO : If haptic disabled, then return
        
        switch (count)
        {
            case <= 0:
                return;
            case 1:
                OnVibrate(type);
                return;
            default:
                CoroutineHelper.StartCoroutine(CO_MultipleVibration(type, count, interval));
                break;
        }
    }
    
    public void StartContinuousVibration(Define.EHapticType type = Define.EHapticType.Medium, float interval = DEFAULT_INTERVAL)
    {
        // TODO : If haptic disabled, then return

        StopContinuousVibration();
        _isVibrating = true;
        _vibrationCoroutine = CoroutineHelper.StartCoroutine(CO_ContinuousVibration(type, interval));
    }
    
    public void StopContinuousVibration()
    {
        // TODO : If haptic disabled, then return

        _isVibrating = false;
        if (_vibrationCoroutine == null) return;
        
        CoroutineHelper.StopCoroutine(_vibrationCoroutine);
        _vibrationCoroutine = null;
    }
    
    // ----- Private
    private void OnVibrate(Define.EHapticType type)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrate(type);
#elif UNITY_IOS && !UNITY_EDITOR
        iOSVibrate(type);
#endif
    }
    
    private void AndroidVibrate(Define.EHapticType type)
    {
        if (Application.platform != RuntimePlatform.Android) return;
        
        var vibrator = GetVibrator();
        if (vibrator == null) return;

        switch (type)
        {
            case Define.EHapticType.Light:
                vibrator.Call(VIBRATE, 20L);
                break;
            case Define.EHapticType.Medium:
                vibrator.Call(VIBRATE, 40L);
                break;
            case Define.EHapticType.Heavy:
                vibrator.Call(VIBRATE, 80L);
                break;
            default:
                Debug.LogError($"[HapticManager.AndroidVibrate] type: {type} is not defined.");
                break;
        }
    }

    private void iOSVibrate(Define.EHapticType type = Define.EHapticType.Medium)
    {
#if UNITY_IOS
        switch (type)
        {
            case Define.EHapticType.Light:
                iOSHapticFeedback(0);
                break;
            case Define.EHapticType.Medium:
                iOSHapticFeedback(1);
                break;
            case Define.EHapticType.Heavy:
                iOSHapticFeedback(2);
                break;
        }
#endif
    }
    
    private AndroidJavaObject GetVibrator()
    {
        if (_vibrator == null)
        {
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            _vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
        }
        return _vibrator;
    }
    
    // --------------------------------------------------
    // Functions - Coroutine
    // --------------------------------------------------
    private IEnumerator CO_MultipleVibration(Define.EHapticType type, int count, float interval, Action doneCallback = null)
    {
        for (var i = 0; i < count; i++)
        {
            OnVibrate(type);
            if (i < count - 1)
                yield return _waitInterval;
            
            doneCallback?.Invoke();
        }
    }
    
    private IEnumerator CO_ContinuousVibration(Define.EHapticType type, float interval, Action doneCallback = null)
    {
        while (_isVibrating)
        {
            OnVibrate(type);
            yield return _waitInterval;
        }
        
        doneCallback?.Invoke();
    }

#if UNITY_IOS
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void iOSHapticFeedback(int style);
#endif
}