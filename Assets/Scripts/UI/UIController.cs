using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI blinkingText;
    private CanvasGroup canvasGroup;
    public float blinkInterval = 0.5f; // Blink 되는 간격
    void Start()
    {
        canvasGroup = blinkingText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = blinkingText.gameObject.AddComponent<CanvasGroup>();
        }
        StartCoroutine("Blink");
    }


    // 텍스트 깜빡이게 하는 효과
    IEnumerator Blink()
    {
        while (true)
        {
            for (float t = 0; t < blinkInterval; t += Time.deltaTime)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / blinkInterval);
                yield return null;
            }

            canvasGroup.alpha = 0f;

            for (float t = 0; t < blinkInterval; t += Time.deltaTime)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / blinkInterval);
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }
    }
}
