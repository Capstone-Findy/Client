using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Button Blink")]
    public TextMeshProUGUI blinkingText;
    public Button blinkingButton;
    public float blinkInterval = 0.5f;

    [Header("Arrow Blink")]
    public Image[] arrows;
    public float arrowInterval = 0.6f;
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
        if (arrows != null && arrows.Length > 0)
        {
            foreach (var arrow in arrows)
            {
                if (arrow != null)
                {
                    CanvasGroup cg = arrow.GetComponent<CanvasGroup>();
                    if (cg == null)
                    {
                        cg = arrow.gameObject.AddComponent<CanvasGroup>();
                    }
                    cg.alpha = 0f;
                    arrow.gameObject.SetActive(true);
                }
            }
            StartCoroutine(ShowArrowsLoop());
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

    IEnumerator ShowArrowsLoop()
    {
        if (arrows == null || arrows.Length == 0) yield break;
        while (true)
        {
            for (int i = 0; i < arrows.Length; i++)
            {
                if (arrows[i] != null)
                {
                    CanvasGroup cg = arrows[i].GetComponent<CanvasGroup>();
                    if (cg != null)
                    {
                        yield return StartCoroutine(Fade(cg, 0f, 1f, arrowInterval));
                    }
                }
            }
            float elapsed = 0f;
            while (elapsed < arrowInterval)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / arrowInterval);
                foreach (var arrow in arrows)
                {
                    if (arrow != null)
                    {
                        CanvasGroup cg = arrow.GetComponent<CanvasGroup>();
                        if (cg != null)
                        {
                            cg.alpha = alpha;
                        }
                    }
                }
                yield return null;
            }
            foreach (var arrow in arrows)
            {
                if (arrow != null)
                {
                    CanvasGroup cg = arrow.GetComponent<CanvasGroup>();
                    if (cg != null)
                    {
                        cg.alpha = 0f;
                    }
                }
            }
        }
    }
    IEnumerator Fade(CanvasGroup cg, float from, float to, float duration)
    {
        float elapsed = 0f;
        cg.alpha = from;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        cg.alpha = to;
    }
}
