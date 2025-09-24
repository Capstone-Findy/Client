using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI blinkingText;
    public Button blinkingButton;
    public float blinkInterval = 0.5f; // Blink 되는 간격
    void Start()
    {
        if (blinkingText != null)
        {
            CanvasGroup textCanvasGroup = blinkingText.GetComponent<CanvasGroup>();
            if (textCanvasGroup == null)
            {
                textCanvasGroup = blinkingText.gameObject.AddComponent<CanvasGroup>();
            }
            StartCoroutine(Blink(textCanvasGroup));
        }
        if (blinkingButton != null)
        {
            CanvasGroup buttonCanvasGroup = blinkingButton.GetComponent<CanvasGroup>();
            if (buttonCanvasGroup == null)
            {
                buttonCanvasGroup = blinkingButton.gameObject.AddComponent<CanvasGroup>();
            }
            StartCoroutine(Blink(buttonCanvasGroup));
        }
    }

    IEnumerator Blink(CanvasGroup canvasGroup)
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
