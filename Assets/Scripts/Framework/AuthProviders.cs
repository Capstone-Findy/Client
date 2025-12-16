using System;


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
            // TODO: authorization code 획득
            onError?.Invoke("Error");
        }
    }
}
