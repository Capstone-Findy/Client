using System;
using TMPro;
// using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_Toast : MonoBehaviour
{
    // --------------------------------------------------
    // Components
    // --------------------------------------------------
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI textMessage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image imageBackground;
    
    // --------------------------------------------------
    // Variables
    // --------------------------------------------------
    private Action _onComplete;
    // private Sequence _sequence;
    
    // --------------------------------------------------
    // Functions - Nomal
    // --------------------------------------------------
    public void Init(ToastData toastData, Action doneCallback)
    {
        _onComplete += toastData.OnComplete;
        _onComplete += doneCallback;
        
        textMessage.text = toastData.Message;
        textMessage.alignment = TextAlignmentOptions.Center;
        
        rectTransform.anchorMin = toastData.Position.AnchorMin;
        rectTransform.anchorMax = toastData.Position.AnchorMax;
        rectTransform.pivot = toastData.Position.Pivot;
        rectTransform.anchoredPosition = toastData.Position.Offset;
        
        rectTransform.localScale = Vector3.zero;
        canvas.sortingOrder = 10000;
        
        // TODO : To use this script, import DoTween package
        // _sequence = DOTween.Sequence();
        // _sequence.Append(rectTransform.DOScale(1.15f, toastData.ScaleDuration).SetEase(Ease.InCubic));
        // _sequence.Append(rectTransform.DOScale(1f, toastData.ScaleDuration).SetEase(Ease.InCubic));
        // _sequence.AppendInterval(toastData.DisplayDuration);
        // _sequence.Append(rectTransform.DOScale(0f, toastData.ScaleDuration).SetEase(Ease.InCubic));
        // _sequence.OnComplete(() =>
        // {
        //     _onComplete?.Invoke();
        //     Destroy(gameObject);
        // });
        // _sequence.SetLink(gameObject);
        // _sequence.Play();
    }
}