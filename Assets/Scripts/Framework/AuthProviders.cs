using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;


namespace Findy.Auth
{
    public interface IAuthProvider
    {
        string ProviderName { get; }
        void GetAuthorizationCode(Action<string> onSuccess, Action<string> onError);
    }

    public class GoogleAuthProvider : IAuthProvider
    {
        public string ProviderName => "google";

        public void GetAuthorizationCode(Action<string> onSuccess, Action<string> onError)
        {
            try
            {
                PlayGamesPlatform.Instance.Authenticate(status =>
                {
                    if (status == SignInStatus.Success)
                    {
                        PlayGamesPlatform.Instance.RequestServerSideAccess(
                            forceRefreshToken: false,
                            code =>
                            {
                                if (!string.IsNullOrEmpty(code))
                                    onSuccess?.Invoke(code);
                                else
                                    onError?.Invoke("Authorization code is empty");
                            });
                    }
                    else
                    {
                        onSuccess?.Invoke($"GPGS Sign-In failed: {status}");
                    }
                });
            }
            catch (Exception ex)
            {
                onError?.Invoke($"GoogleAuthProvider Exception: {ex.Message}");
            }
        }
    }

    public class KakaoAuthProvider : IAuthProvider
    {
        public string ProviderName => "kakao";

        public void GetAuthorizationCode(Action<string> onSuccess, Action<string> onError)
        {
            string clientId = ""; // Rest API Key
            string redirectUri = ""; // Redirect URI
            string url = $"https://kauth.kakao.com/oauth/authorize?client_id={clientId}&redirect_uri={redirectUri}&response_type=code";

#if UNITY_ANDROID || UNITY_IOS
            Application.OpenURL(url);
#else
            Application.OpenURL(url);
#endif
            onError?.Invoke("Kakao login flow must be completed in WebView or plugin.");
        }
    }
}
