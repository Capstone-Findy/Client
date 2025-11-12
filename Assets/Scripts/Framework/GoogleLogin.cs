using UnityEngine;
using UnityEngine.UI;

public class GoogleLogin : MonoBehaviour
{
    public Button loginButton;
    public GameObject loadingSpinner;
    void Awake()
    {
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnClickGoogleLogin);
        }
        SetLoading(false);
    }
    public void OnClickGoogleLogin()
    {
        SetLoading(true);

        var provider = new Findy.Auth.GoogleAuthProvider();
        DataManager.instance.AuthenticateWithProvider(
            provider: provider,
            codeVerifier: null,
            onSuccess: () =>
            {
                SetLoading(false);
            },
            onError: (code, msg) =>
            {
                Debug.Log("Error!");
                SetLoading(false);
            });
    }
    private void SetLoading(bool isOn)
    {
        if (loadingSpinner != null) loadingSpinner.SetActive(isOn);
        if(loginButton != null) loginButton.interactable = !isOn;
    }

}
