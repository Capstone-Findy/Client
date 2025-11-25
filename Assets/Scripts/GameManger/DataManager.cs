using System;
using System.Collections;
using System.Text;
using Findy.Define;
using Findy.JsonHelper;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager : MonoBehaviour
{
    public static DataManager instance { get; private set; }

    //----- Setting -----//
    private const string BASE_URL = "http://54.116.10.1:8080"; // 서버 주소
    private const string POST_SIGNUP_URL = "/sign-up";
    private const string POST_LOGIN_URL = "/sign-in";
    private const string GET_USER_INFO_PATH = "/auth/user";
    private const string HEART_UPDATE_PATH = "/auth/user/heart/1";
    private const string POST_GAME_RESULT_PATH = "/auth/origin/result";
    
    private const string PLAYERPREFS_JWT_KEY = "APP_JWT";

    //----- Event/Callback -----//
    public event Action OnAuthExpired;

    //----- Key -----//
    private const string UNLOCK_KEY_PREFIX = "UnlockedStageIndex_";
    private const string UNLOCK_COUNTRY_KEY = "UnlockedCountryIndex";
    private const string BEST_TIME_KEY_PREFIX = "StageBestTime_";

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
    private IEnumerator CoPostJson(string path, object bodyObj, Action<string> onSuccess, Action<long, string> onError)
    {
        string json = JsonUtility.ToJson(bodyObj);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        using(UnityWebRequest req = new UnityWebRequest(BASE_URL + path, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(jsonBytes);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if(req.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(req.downloadHandler.text);
            }
            else
            {
                string errorMsg = string.IsNullOrEmpty(req.downloadHandler.text) ? req.error : req.downloadHandler.text;
                onError?.Invoke(req.responseCode, errorMsg);
            }
        }
    }

    private void HandleAuthExpired()
    {
        Debug.LogWarning("[Auth] JWT expired or invalid. Clearing token.");
        ClearJwt();
        OnAuthExpired?.Invoke();
    }

    public void SignUp(string name, string file, string email, string password, Action onSuccess, Action<long, string> onError)
    {
        var payload = new SignUpRequestDto
        {
            name = name,
            file = file,
            email = email,
            password = password
        };

        StartCoroutine(CoPostJson(POST_SIGNUP_URL, payload,
            onSuccess: (txt) =>
            {
                Debug.Log($"[API] SignUp Success: {txt}");
                onSuccess?.Invoke();
            },
            onError: (code, msg) =>
            {
                Debug.LogError($"[API] SignUp Failed: Code {code}, Msg {msg}");
                onError?.Invoke(code, msg);
            }
        ));
    }

    public void Login(string email, string password, bool rememberMe, Action onSuccess, Action<long, string> onError)
    {
        var payload = new LoginRequestDto
        {
            email = email,
            password = password,
            rememberMe = rememberMe
        };

        StartCoroutine(CoPostJson(POST_LOGIN_URL, payload,
            onSuccess: (txt) =>
            {
                var ar = JsonUtility.FromJson<AuthResponse>(JsonHelper.WrapJsonIfBare(txt));
                if(ar != null && !string.IsNullOrEmpty(ar.token))
                {
                    SaveJwt(ar.token);
                    Debug.Log($"[API] Login Success. JWT Saved.");
                    GetUserInfo(
                        onSuccess: (userData) =>
                        {
                            GameManager.instance.currentUserData = userData;
                            Debug.Log($"User Data Fetched for: {userData.name}, Money: {userData.money}");
                            onSuccess?.Invoke();
                        },
                        onError: (code, msg) =>
                        {
                            onError?.Invoke(code, $"로그인은 성공했으나 유저 정보 로드 실패: {msg}");
                        }
                    );
                }
                else
                {
                    onError?.Invoke(0, "로그인 응답에서 토큰을 찾을 수 없습니다.");
                }
            },
            onError: (code, msg) =>
            {
                Debug.LogError($"[API] Login Failed: Code {code}, Msg {msg}");
                onError?.Invoke(code, msg);
            }
        ));
    }

    public void GetUserInfo(Action<Findy.Define.UserDataDto> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(CoGetAuthorized(GET_USER_INFO_PATH,
            onSuccess: (txt) =>
            {
                try
                {
                    var resp = JsonUtility.FromJson<Findy.Define.UserDataDto>(txt);

                    if(resp != null && !string.IsNullOrEmpty(resp.name))
                    {
                        Debug.Log($"[API] User Info Success: {resp.name}");
                        onSuccess?.Invoke(resp);
                    }
                    else
                    {
                        onError?.Invoke(0, "유저 정보 응답 형식이 잘못되었습니다.");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[API] User Info JSON Parse Error: {e.Message}");
                    onError?.Invoke(0, "유저 정보 파싱 오류");
                }
            },
            onError: (code, msg) =>
            {
                Debug.LogError($"[API] Get User Info Failed: Code {code}, Msg {msg}");
                onError?.Invoke(code, msg);
            }
        ));
    }

    public void UpdateHeart(int delta, Action onSuccess = null, Action<long, string> onError = null)
    {
        var payload = new HeartUpdateDto {heart = delta};
        StartCoroutine(CoPostJsonAuthorized(HEART_UPDATE_PATH, payload,
            onSuccess: (txt) => onSuccess?.Invoke(),
            onError: (code, msg) => onError?.Invoke(code, msg)));
    }

    public void UploadGameResult(GameResultDto result, Action onSuccess = null, Action<long, string> onError = null)
    {
        StartCoroutine(CoPostJsonAuthorized(POST_GAME_RESULT_PATH, result,
            onSuccess: (txt) =>
            {
                Debug.Log($"[API] Game Result Upload Success: {txt}");
                onSuccess?.Invoke();
            },
            onError: (code, msg) =>
            {
                Debug.LogError($"[API] Game Result Upload Failed: Code {code}, Msg {msg}");
                onError?.Invoke(code, msg);
            }));
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

    //----- Stage Best Time -----//
    public static float? GetLocalBestClearTime(string countryName, int stageIndex)
    {
        string key = BEST_TIME_KEY_PREFIX + countryName + "_" + stageIndex;
        if(PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetFloat(key);
        }
        return null;
    }
    public static void SaveLocalBestClearTime(string countryName, int stageIndex, float clearTime)
    {
        string key = BEST_TIME_KEY_PREFIX + countryName + "_" + stageIndex;
        PlayerPrefs.SetFloat(key, clearTime);
        PlayerPrefs.Save();
    }
}