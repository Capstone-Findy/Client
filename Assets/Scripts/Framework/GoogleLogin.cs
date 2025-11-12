using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.UI;

public class GoogleLogin : MonoBehaviour
{
    public Button loginButton;
    public GameObject loadingSpinner;
    void Awake()
    {
        PlayGamesPlatform.Activate();
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnClickGoogleLogin);
        }
        SetLoading(false);
    }
    public void OnClickGoogleLogin()
    {
        SetLoading(true);

        PlayGamesPlatform.Instance.Authenticate(status =>
        {
            if (status == SignInStatus.Success)
            {
                var scopes = new List<AuthScope> { AuthScope.OPEN_ID, AuthScope.PROFILE, AuthScope.EMAIL };
                PlayGamesPlatform.Instance.RequestServerSideAccess(false, scopes, resp =>
                {
                    string authCode = resp.GetAuthCode();
                    if (string.IsNullOrEmpty(authCode))
                    {
                        Debug.LogError("AuthCode is null/empty.");
                        SetLoading(false);
                        return;
                    }

                    DataManager.instance.AuthenticateWithServer(authCode, onSuccess: () =>
                    {
                        Debug.Log("[Auth] Backend JWT OK");
                        SetLoading(false);
                    },
                    onError: (code, msg) =>
                    {
                        Debug.LogError($"[Auth] Backend exchange failed: {code} {msg}");
                        SetLoading(false);
                    });
                });
            }
            else
            {
                Debug.LogError("GPGS Sign-In failed: " + status);
                SetLoading(false);
            }
        });
    }
    private void SetLoading(bool isOn)
    {
        if (loadingSpinner != null) loadingSpinner.SetActive(isOn);
        if(loginButton != null) loginButton.interactable = !isOn;
    }

}
