// ----- C#
using System;
using System.Collections;

// ----- Unity
using UnityEngine;

#if UNITY_ANDROID
using Google.Play.Review;
#endif

public class ReviewSystem
{
    private static bool? _isAlreadyReview = null;
    private static bool IsAlreadyReview
    {
        get
        {
            if (_isAlreadyReview == null)
            {
                // TODO : Load _isAlreadyReview From SaveData
            }
            return _isAlreadyReview.Value;
        }
        set
        {
            if (_isAlreadyReview != value)
            {
                _isAlreadyReview = value;
                // TODO : Save _isAlreadyReview
            }
        }
    }
    
    public void Open()
    {
        if (!IsAlreadyReview)
        {
            IsAlreadyReview = true;
#if UNITY_ANDROID
            RequestGooglePlayStoreReview();
#elif UNITY_IOS
            RequestAppStoreReview();
#endif
        }
        else
        {
            Debug.Log("[ReviewSystem.Open] Already Review");
        }
    }

    private static void RequestGooglePlayStoreReview()
    {
#if UNITY_ANDROID
        CoroutineHelper.StartCoroutine(RequestGooglePlayStoreReviewCoroutine());
#endif
    }

    private static IEnumerator RequestGooglePlayStoreReviewCoroutine(Action onFinished = null)
    {
#if UNITY_ANDROID
        ReviewManager reviewManager = new ReviewManager();
        var requestFlowOperation = reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogError("[ReviewSystem.Open] RequestFlowOperation" + requestFlowOperation.Error);
            OpenGooglePlayStorePage();
            onFinished?.Invoke();
            onFinished = null;
            yield break;
        }
        PlayReviewInfo playReviewInfo = requestFlowOperation.GetResult();

        // Managers.Ads.IsThirdPartyOn = true;
        var launchFlowOperation = reviewManager.LaunchReviewFlow(playReviewInfo);
        yield return launchFlowOperation;
        playReviewInfo = null;
        // Managers.Ads.IsThirdPartyOn = false;
        
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogError("[ReviewSystem.Open] LaunchFlowOperation: " + launchFlowOperation.Error);
            OpenGooglePlayStorePage();
            onFinished?.Invoke();
            onFinished = null;
            yield break;
        }
        Debug.Log("[ReviewSystem.Open] Open Review Popup");
#endif
        onFinished?.Invoke();
        onFinished = null;
        yield break;
    }

    private static void OpenGooglePlayStorePage()
    {
#if UNITY_ANDROID
        Application.OpenURL($"market://details?id={Application.identifier}");
#endif
    }

    private static void RequestAppStoreReview()
    {
#if UNITY_IOS
        UnityEngine.iOS.Device.RequestStoreReview();
#endif
    }
}