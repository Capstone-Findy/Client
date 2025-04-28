// ----- C#
using System;
using System.Collections;
using System.Numerics;

// ----- Unity
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = UnityEngine.Debug;
// using DG.Tweening;

// ----- User Defined
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Utils
{
    // --------------------------------------------------
    // Variables
    // --------------------------------------------------
    private const float SHAKE_DURATION = 0.2f;
    private const float SHAKE_STRENGTH = 1.5f;
    private const int SHAKE_VIBRATO = 50;
    private const float SHAKE_RANDOMNESS = 1f;
    

    public static void QuitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    public static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        return go.GetComponent<T>() ?? go.AddComponent<T>();
    }

    public static bool IsClickOutOfUI()
    {
        return Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject();
    }
    
    public static bool IsTouchOutOfUI()
    {
        if (Input.touchCount <= 0) return false;
        
        for (var i = 0; i < Input.touchCount; i++)
        {
            var touch = Input.GetTouch(i);
            if (touch.phase != TouchPhase.Began || EventSystem.current.IsPointerOverGameObject(touch.fingerId)) continue;
            return true;
        }

        return false;
    }
    
    public static IEnumerator NumberCountingEffect(TextMeshProUGUI targetText, BigInteger startValue, BigInteger targetValue, string prefix = "", float duration = 2f, 
        Action doneCallback = null)
    {
        bool isDecrease = startValue > targetValue;
        if (isDecrease)
        {
            (startValue, targetValue) = (targetValue, startValue);
        }
        var offset = (targetValue - startValue) / (BigInteger)duration;
        
        while (startValue <= targetValue)
        {
            if (isDecrease)
            {
                targetValue -= MultiplyBigIntegerByFloat(offset, Time.deltaTime);
                targetText.text = prefix + AddThousandSeparators(targetValue);
            }
            else
            {
                startValue += MultiplyBigIntegerByFloat(offset, Time.deltaTime);
                targetText.text = prefix + AddThousandSeparators(startValue);
            }
            yield return null;
        }
        
        if (isDecrease)
            targetText.text = prefix + AddThousandSeparators(startValue);
        else
            targetText.text = prefix + AddThousandSeparators(targetValue);
        
        doneCallback?.Invoke();
    }
    
    public static BigInteger MultiplyBigIntegerByFloat(BigInteger numBigInteger, float numFloat, float digits = 10000f)
    {
        var roundedFloat = Mathf.Round(numFloat * digits) / digits;
        var scaledFloat = Mathf.RoundToInt(roundedFloat * digits); 
        var result = numBigInteger * scaledFloat;
        
        result /= (int)digits;
        result = BigInteger.Max(result, 1);

        return result;
    }


    public static string GetColonTimeForm(float time)
    {
        return TimeSpan.FromSeconds(time).ToString(@"dd\:mm\:ss");
    }

    public static string AddThousandSeparators<T>(T value)
    {
        var formattedValue = $"{value:#,##0.##}";
        return formattedValue == "" ? "0" : formattedValue;
    }
}