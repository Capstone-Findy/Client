// ----- C#
using System;
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;

public class CoroutineHelper : MonoBehaviour
{
    // --------------------------------------------------
    // Variables
    // --------------------------------------------------
    private static MonoBehaviour monoInstance;
    public static WaitForEndOfFrame WaitForEndOfFrame { get; } = new();
    public static WaitForFixedUpdate WaitForFixedUpdate { get; } = new();
    private static Dictionary<float, WaitForSeconds> waitForSecondsCache = new();
    
    // --------------------------------------------------
    // Functions
    // --------------------------------------------------
    public static void Init(Action callback = null)
    {
        if (monoInstance != null)
        {
            callback?.Invoke();
            return;
        }
        
        monoInstance = new GameObject(
            $"[{nameof(CoroutineHelper)}]"
        ).AddComponent<CoroutineHelper>();
        DontDestroyOnLoad(monoInstance.gameObject);
        callback?.Invoke();
    }

    public new static Coroutine StartCoroutine(IEnumerator coroutine)
    {
        return monoInstance.StartCoroutine(coroutine);
    }

    public new static void StopCoroutine(Coroutine coroutine)
    {
        monoInstance.StopCoroutine(coroutine);
    }
    
    public static WaitForSeconds GetWaitForSeconds(float seconds)
    {
        if (!waitForSecondsCache.TryGetValue(seconds, out var wait))
        {
            wait = new WaitForSeconds(seconds);
            waitForSecondsCache[seconds] = wait;
        }
        return wait;
    }
}
