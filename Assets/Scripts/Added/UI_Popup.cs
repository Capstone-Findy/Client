// ----- C#
using System;
using System.Collections;

// ----- Unity
// using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/* 팝업을 만들고 싶으면, UI_Popup을 상속받는 UI_NoticePopup과 같은 스크립트를 만들어 사용하세요.
 */
public class UI_Popup : MonoBehaviour
{
    // --------------------------------------------------
    // Components   
    // --------------------------------------------------
    [Header("[Option]")]
    [SerializeField] protected bool isCloseDim = true;
    [SerializeField] protected bool isPlaySound = true;
    [SerializeField] protected bool isPlayAnimation = true;
    
    [Space(1.5f)] [Header("[Components]")]
    [SerializeField] protected RectTransform frame = null;
    [SerializeField] protected Image imageDim = null;
    [SerializeField] protected Button buttonClose = null;
    
    // --------------------------------------------------
    // Event
    // --------------------------------------------------
    public event Action OnBeforeCloseAction = null;
    public event Action OnAfterCloseAction = null;
    
    // --------------------------------------------------
    // Properties
    // --------------------------------------------------
    public bool IsPlayAnimation => isPlayAnimation;
    public bool IsPlaySound => isPlaySound;
    
    // --------------------------------------------------
    // Variables
    // --------------------------------------------------
    private bool _isClosing = false;

    // --------------------------------------------------
    // Functions - Event
    // --------------------------------------------------
    private void Awake()
    {
        Managers.UI.SetCanvas(gameObject, true);
        BindBackGroundAction();
        BindButtonCloseAction();
        OnAwake();
    }

    public virtual void OnAwake()
    {
        PlayAnimation(true);
        PlaySound();
    }

    // --------------------------------------------------
    // Functions - Nomal
    // --------------------------------------------------
    // ----- Public
    public virtual void Init()
    {
        Managers.UI.SetCanvas(gameObject,true);
    }

    public virtual void ClosePopupUI(bool isCloseImmediately = false)
    {
        OnBeforeCloseAction?.Invoke();
        if (isCloseImmediately)
            Managers.UI.ClosePopupUI(this);
        else if (isPlayAnimation && frame)
            PlayAnimation(false);
        else
            Managers.UI.ClosePopupUI(this);
        OnAfterCloseAction?.Invoke();
    }
    
    public virtual void Refresh()
    {
    
    }

    public virtual void PlaySound()
    {
        if (isPlaySound)
        {
            // TODO: PlaySound
        }
    }

    public virtual void PlayAnimation()
    {
        if (!isPlayAnimation || !frame) return;
        
        frame.transform.localScale = Vector3.zero;            
        // var sequence = DOTween.Sequence();
        // sequence.Append(frame.transform.DOScale(new Vector3(1.15f, 1.15f, 1.15f), 0.1f))
        //     .Append(frame.transform.DOScale(Vector3.one, 0.1f)); 
        // sequence.Play();
    }

    public virtual void PlayAnimation(bool isShow)
    {
        if (!isPlayAnimation || !frame) return;
        
        if (isShow)
        {
            frame.transform.localScale = Vector3.zero;            
            // var sequence = DOTween.Sequence();
            // sequence.Append(frame.transform.DOScale(new Vector3(1.15f, 1.15f, 1.15f), 0.2f))
            //     .Append(frame.transform.DOScale(Vector3.one, 0.05f)); 
            // sequence.Play();
        }
        else
            StartCoroutine(Co_Close());
    }
    
    public void OnOffDim(bool isOn)
    {
        if (!imageDim)
            return;
        
        imageDim.gameObject.SetActive(isOn);
    }
    
    // ----- Private
    private void BindBackGroundAction()
    {
        if (isCloseDim == false || imageDim == null)
            return;

        imageDim?.gameObject.AddUIEvent(data =>
        {
            if (data.pointerCurrentRaycast.gameObject == imageDim.gameObject)
                ClosePopupUI();
        });
    }

    private void BindButtonCloseAction()
    {
        if (!buttonClose)
            return;
        buttonClose.onClick.RemoveAllListeners();
        buttonClose.onClick.AddListener(() => ClosePopupUI());
    }
    
    // --------------------------------------------------
    // Coroutine
    // --------------------------------------------------
    private IEnumerator Co_Close()
    {
        if (_isClosing) yield break;
        _isClosing = true;
        
        frame.transform.localScale = Vector3.one;            
        // var sequence = DOTween.Sequence();
        // sequence.Append(frame.transform.DOScale(new Vector3(1.075f, 1.075f, 1.075f), 0.05f))
        //     .Append(frame.transform.DOScale(Vector3.zero, 0.125f)); 
        // sequence.Play();

        yield return new WaitForSeconds(0.175f);
    
        _isClosing = false;
        Managers.UI.ClosePopupUI(this);
    }
}
