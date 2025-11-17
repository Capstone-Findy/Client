using System;
using System.Collections;
using System.Text;
using Findy.Auth;
using Findy.Define;
using Findy.JsonHelper;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager : MonoBehaviour
{
    public static DataManager instance { get; private set; }

    //----- Setting -----//
    private const string BASE_URL = ""; // 서버 주소
    private const string AUTH_EXCHANGE_PATH = "/api/auth/google"; // authCode 교환 엔드포인트
    private const string SAVE_PROGRESS_PATH = "/api/me/progress";  // 진행도 저장/갱신
    private const string GET_PROGRESS_PATH = "/api/me/progress";
    
    private const string PLAYERPREFS_JWT_KEY = "APP_JWT";

    //----- Event/Callback -----//
    public event Action OnAuthExpired;

    //----- Key -----//
    private const string UNLOCK_KEY_PREFIX = "UnlockedStageIndex_";
    private const string UNLOCK_COUNTRY_KEY = "UnlockedCountryIndex";

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //----- JWT Control -----//
    public bool HasJwt()
    {
        return !string.IsNullOrEmpty(PlayerPrefs.GetString(PLAYERPREFS_JWT_KEY, ""));
    }
    public string GetJwt()
    {
        return PlayerPrefs.GetString(PLAYERPREFS_JWT_KEY, "");
    }
    private void SaveJwt(string token)
    {
        PlayerPrefs.SetString(PLAYERPREFS_JWT_KEY, token);
        PlayerPrefs.Save();
    }
    public void ClearJwt()
    {
        PlayerPrefs.DeleteKey(PLAYERPREFS_JWT_KEY);
        PlayerPrefs.Save();
    }

    //----- JWT Exchange -----//
    public void AuthenticateWithProvider(
        IAuthProvider provider,
        string codeVerifier,
        Action onSuccess,
        Action<long, string> onError)
    {
        provider.GetAuthorizationCode(
            onSuccess: (authCode) =>
            {
                StartCoroutine(CoExchangeCode(provider.ProviderName, "", codeVerifier, onSuccess, onError));
            },
            onError: (msg) => onError?.Invoke(0, msg)
        );
    }
    
    private IEnumerator CoExchangeCode(
        string provider,
        string authCode,
        string codeVerifier,
        Action onSuccess,
        Action<long, string> onError)
    {
        string url = BASE_URL + $"{AUTH_EXCHANGE_PATH}/{provider}";
        WWWForm form = new WWWForm();
        form.AddField("authCode", authCode);
        if (!string.IsNullOrEmpty(codeVerifier))
            form.AddField("codeVerifier", codeVerifier);
        
        using (UnityWebRequest req = UnityWebRequest.Post(url, form))
        {
        req.timeout = 15;
        req.SetRequestHeader("Accept", "application/json");
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            var json = req.downloadHandler.text;
            AuthResponse ar = JsonUtility.FromJson<AuthResponse>(JsonHelper.WrapJsonIfBare(json));
            if (ar != null && !string.IsNullOrEmpty(ar.token))
            {
                SaveJwt(ar.token);
                onSuccess?.Invoke();
            }
            else
            {
                onError?.Invoke(req.responseCode, "Token missing in response");
            }
        }
        else
        {
            onError?.Invoke(req.responseCode, $"{req.error} | {req.downloadHandler.text}");
        }
    }
}

    public void SaveProgress(string country, int stage, float clearTime, Action onSuccess = null, Action<long, string> onError = null)
    {
        var payload = new StageProgressDto { country = country, stage = stage, clearTime = clearTime };
        StartCoroutine(CoPostJsonAuthorized(SAVE_PROGRESS_PATH, payload,
            onSuccess: (txt) => onSuccess?.Invoke(),
            onError: (code, msg) => onError?.Invoke(code, msg)));
    }
    public void GetProgress(Action<string> onSuccess, Action<long, string> onError = null)
    {
        StartCoroutine(CoGetAuthorized(GET_PROGRESS_PATH, onSuccess, onError));
    }
    private IEnumerator CoGetAuthorized(string path, Action<string> onSuccess, Action<long, string> onError)
    {
        if (!HasJwt())
        {
            onError?.Invoke(0, "No JWT");
            yield break;
        }

        using (UnityWebRequest req = UnityWebRequest.Get(BASE_URL + path))
        {
            req.SetRequestHeader("Authorization", "Bearer " + GetJwt());
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(req.downloadHandler.text);
            }
            else
            {
                if (req.responseCode == 401)
                {
                    HandleAuthExpired();
                }
                onError?.Invoke(req.responseCode, req.error);
            }
        }
    }

    // ========= 공통: Authorized POST(JSON) =========
    private IEnumerator CoPostJsonAuthorized(string path, object bodyObj,
                                             Action<string> onSuccess, Action<long, string> onError)
    {
        if (!HasJwt())
        {
            onError?.Invoke(0, "No JWT");
            yield break;
        }

        string json = JsonUtility.ToJson(bodyObj);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest req = new UnityWebRequest(BASE_URL + path, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(jsonBytes);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Authorization", "Bearer " + GetJwt());

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(req.downloadHandler.text);
            }
            else
            {
                if (req.responseCode == 401)
                {
                    HandleAuthExpired();
                }
                onError?.Invoke(req.responseCode, req.error);
            }
        }
    }

    public void GetProgressForStage(string country, int stage, Action<float?> onSuccess, Action<long, string> onError = null)
    {
        string qs = $"?country={UnityWebRequest.EscapeURL(country)}&stage={stage}";
        StartCoroutine(CoGetAuthorized(GET_PROGRESS_PATH + qs,
            onSuccess: (txt) =>
            {
                try
                {
                    var resp = JsonUtility.FromJson<StageProgressDto>(JsonHelper.WrapJsonIfObject(txt));
                    if (resp != null && !string.IsNullOrEmpty(resp.country))
                    {
                        onSuccess?.Invoke(resp.clearTime);
                    }
                    else
                    {
                        onSuccess?.Invoke(null);
                    }
                }
                catch
                {
                    onSuccess?.Invoke(null);
                }
            },
            onError: (code, msg) =>
            {
                if (code == 404)
                {
                    onSuccess?.Invoke(null);
                    return;
                }
                onError?.Invoke(code, msg);
            }
        ));
    }

    private void HandleAuthExpired()
    {
        Debug.LogWarning("[Auth] JWT expired or invalid. Clearing token.");
        ClearJwt();
        OnAuthExpired?.Invoke();
    }

    //----- Unlock Stage ----- //

    public static int GetUnlockedStageIndex(string countryName)
    {
        return PlayerPrefs.GetInt(UNLOCK_KEY_PREFIX + countryName, 0);
    }

    public static void UnlockNextStage(string countryName, int clearedStageIndex)
    {
        int currentlyUnlockedIndex = GetUnlockedStageIndex(countryName);
        int nextStageIndex = clearedStageIndex + 1;

        if (nextStageIndex > currentlyUnlockedIndex)
        {
            PlayerPrefs.SetInt(UNLOCK_KEY_PREFIX + countryName, nextStageIndex);
            PlayerPrefs.Save();
        }
    }

    //----- Unlock Country -----//
    public static int GetUnlockedCountryIndex()
    {
        return PlayerPrefs.GetInt(UNLOCK_COUNTRY_KEY, 0);
    }
    public static void UnlockNextCountry(int clearedCountryIndex)
    {
        int currentlyUnlockedIndex = GetUnlockedCountryIndex();
        int nextCountryIndex = clearedCountryIndex + 1;

        if(nextCountryIndex > currentlyUnlockedIndex)
        {
            PlayerPrefs.SetInt(UNLOCK_COUNTRY_KEY, nextCountryIndex);
            PlayerPrefs.Save();
        }
    }
}