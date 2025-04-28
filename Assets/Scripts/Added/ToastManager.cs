// ----- C#
using System;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;

public enum EToastPositionType
{
    Top,
    Middle,
    Bottom
}

public struct ToastPosition
{
    public Vector2 AnchorMin;
    public Vector2 AnchorMax;
    public Vector2 Pivot;
    public Vector2 Offset;
}

public class ToastData
{
    public float DisplayDuration { get; set; } = 2.0f;
    public float ScaleDuration { get; set; } = 0.075f;
    public string Message { get; set; }
    public Action OnComplete { get; set; }
    public ToastPosition Position { get; set; }
}

public class ToastManager
{
    // --------------------------------------------------
    // Variables
    // --------------------------------------------------
    // ----- Private
    private Dictionary<EToastPositionType, ToastPosition> _positionDict = new()
    {
        {EToastPositionType.Top, new ToastPosition 
            {
                AnchorMin = new Vector2(0.5f, 1),
                AnchorMax = new Vector2(0.5f, 1),
                Pivot = new Vector2(0.5f, 1),
                Offset = new Vector2(0, -100)
            }
        },
        {EToastPositionType.Middle, new ToastPosition
            {
                AnchorMin = new Vector2(0.5f, 0.5f),
                AnchorMax = new Vector2(0.5f, 0.5f),
                Pivot = new Vector2(0.5f, 0.5f),
                Offset = new Vector2(0, 0)
            }
        },
        {EToastPositionType.Bottom, new ToastPosition
        {
            AnchorMin = new Vector2(0.5f, 0),
            AnchorMax = new Vector2(0.5f, 0),
            Pivot = new Vector2(0.5f, 0),
            Offset = new Vector2(0, 100)
        }}
    };
    
    private Queue<ToastData> _toastQueue = new();
    private bool _isDisplaying = false;
    private GameObject _currentToast;
    
    // ----- Const
    private const string UI_TOAST_PATH = "UI/Common/UI_Toast.prefab";
    private const float DISPLAY_DURATION = 2.0f;
    
    // --------------------------------------------------
    // Functions - Nomal
    // --------------------------------------------------
    // ----- Public
    public void ShowToast(string message, float duration = DISPLAY_DURATION, EToastPositionType position = EToastPositionType.Middle, Action onComplete = null)
    {
        
        var toastData = new ToastData
        {
            Message = message,
            OnComplete = onComplete,
            Position = _positionDict[position],
            DisplayDuration = duration,
        };
        
        _toastQueue.Enqueue(toastData);
        
        // if (!_isDisplaying)
        ProcessNextToast();
    }
    
    // ----- Private
    private void ProcessNextToast()
    {
        if (_toastQueue.Count == 0)
        {
            _isDisplaying = false;
            return;
        }
        
        _isDisplaying = true;
        var toastData = _toastQueue.Dequeue();
        CreateToastUI(toastData);
    }
    
    private void CreateToastUI(ToastData toastData)
    {
        if (_currentToast)
            UnityEngine.Object.Destroy(_currentToast);
        
        // _currentToast = TODO : Instantiate toast prefab (UI_TOAST_PATH, TargetTransform)
        if (_currentToast.TryGetComponent(out UI_Toast toastUI))
            toastUI.Init(toastData, ProcessNextToast);
    }
}