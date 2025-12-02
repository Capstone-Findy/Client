using System;
using System.Collections;
using System.Text;
using Findy.Define;
using Findy.JsonHelper;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager : MonoBehaviour
{
    public static DataManager instance { get; private set; }

    //----- Setting -----//
    private const string BASE_URL = "http://54.116.10.1:8080"; // 서버 주소
    private const string POST_SIGNUP_URL = "/sign-up";
    private const string POST_LOGIN_URL = "/sign-in";
    private const string POST_EMAIL_VALID_PATH = "/valid";
    private const string POST_REFRESH_PATH = "/auth/refresh";
    private const string GET_USER_INFO_PATH = "/auth/user";
    private const string HEART_UPDATE_PATH = "/auth/user/heart/1";
    private const string ITEM_UPDATE_PATH = "/auth/user/item";
    private const string POST_GAME_RESULT_PATH = "/auth/origin/result";
    private const string GET_GAME_SCORE_PATH = "/auth/origin";
    
    private const string PLAYERPREFS_JWT_KEY = "APP_JWT";
    private const string PLAYERPREFS_REFRESH_KEY = "APP_REFRESH_TOKEN";

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

    public bool HasRefreshToken()
    {
        return !string.IsNullOrEmpty(PlayerPrefs.GetString(PLAYERPREFS_REFRESH_KEY, ""));
    }
    public string GetRefreshToken()
    {
        return PlayerPrefs.GetString(PLAYERPREFS_REFRESH_KEY, "");
    }
    private void SaveRefreshToken(string token)
    {
        PlayerPrefs.SetString(PLAYERPREFS_REFRESH_KEY, token);
        PlayerPrefs.Save();
    }
    public void ClearRefreshToken()
    {
        PlayerPrefs.DeleteKey(PLAYERPREFS_REFRESH_KEY);
        PlayerPrefs.Save();
    }

    //----- Tools -----//
    private IEnumerator CoGetAuthorized(string path, Action<string> onSuccess, Action<long, string> onError)
    {
        if (!HasJwt())
        {
            onError?.Invoke(0, "No JWT");
            yield break;
        }

        using (UnityWebRequest req = UnityWebRequest.Get(BASE_URL + path))
        {
            req.SetRequestHeader("Cookie", "OID_AUT=" + GetJwt());
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
            req.SetRequestHeader("Cookie", "OID_AUT=" + GetJwt());
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
                var headers = req.GetResponseHeaders();
                if (headers != null && headers.TryGetValue("Set-Cookie", out string cookieHeader))
                {
                    if (cookieHeader.Contains("OID_AUT="))
                    {
                        int startIndex = cookieHeader.IndexOf("OID_AUT=") + "OID_AUT=".Length;
                        int endIndex = cookieHeader.IndexOf(';', startIndex);
                        if (endIndex == -1) endIndex = cookieHeader.Length; 

                        string accessTokenCookie = cookieHeader.Substring(startIndex, endIndex - startIndex);
                        
                        if (!string.IsNullOrEmpty(accessTokenCookie))
                        {
                            SaveJwt(accessTokenCookie);
                            Debug.Log($"[Auth] Access Token (OID_AUT) extracted and saved.");
                        }
                    }
                }
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
    
    //----- Function -----//
    public void SignUp(string name, string base64File, string email, string password, Action onSuccess, Action<long, string> onError)
    {
        var payload = new SignUpRequestDto
        {
            name = name,
            file = base64File,
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
            LoginResponseWrapper wrapper = JsonUtility.FromJson<LoginResponseWrapper>(txt);
            
            if (HasJwt())
            {
                AuthResponse ar = wrapper?.data; 
                
                if (ar != null && !string.IsNullOrEmpty(ar.refreshToken))
                {
                    if (rememberMe)
                    {
                        SaveRefreshToken(ar.refreshToken); 
                    }
                    else
                    {
                        ClearRefreshToken();
                    }
                }
                
                Debug.Log($"[API] Login Success. Cookie Saved. Proceeding to GetUserInfo.");
                
                GetUserInfo(
                    onSuccess: (userData) =>
                    {
                        GameManager.instance.currentUserData = userData;
                        onSuccess?.Invoke();
                    },
                    onError: (code, msg) =>
                    {
                        onError?.Invoke(code, $"로그인 성공 (쿠키 획득) 후 유저 정보 로드 실패: {msg}");
                    }
                );
            }
            else if (wrapper != null && wrapper.error)
            {
                onError?.Invoke(401, wrapper.message);
            }
            else
            {
                onError?.Invoke(0, "로그인 응답 구조 문제 또는 Access Token 쿠키 획득 실패.");
            }
        },
        onError: (code, msg) =>
        {
            Debug.LogError($"[API] Login Failed: Code {code}, Msg {msg}");
            onError?.Invoke(code, msg);
        }
    ));
}
    public void SendValidationEmail(string email, Action onSuccess, Action<long, string> onError)
    {
        var payload = new EmailValidationDto { email = email };

        StartCoroutine(CoPostJson(POST_EMAIL_VALID_PATH, payload,
            onSuccess: (txt) =>
            {
                Debug.Log($"[API] Validation Email Sent to {email}");
                onSuccess?.Invoke();
            },
            onError: (code, msg) =>
            {
                Debug.LogError($"[API] Failed to Send Validation Email: {code}, {msg}");
                onError?.Invoke(code, msg);
            }));
    }
    public void RefreshToken(Action onSuccess, Action<long, string> onError)
    {
        string currentRefreshToken = GetRefreshToken();
        if(string.IsNullOrEmpty(currentRefreshToken))
        {
            onError?.Invoke(0, "Refresh 토큰이 없습니다.");
            return;
        }
        var payload = new RefreshRequestDto { refreshToken = currentRefreshToken};
        StartCoroutine(CoPostJson(POST_REFRESH_PATH, payload,
            onSuccess: (txt) =>
            {
                var ar = JsonUtility.FromJson<AuthResponse>(txt);

                if(ar != null && !string.IsNullOrEmpty(ar.accessToken) && !string.IsNullOrEmpty(ar.refreshToken))
                {
                    SaveJwt(ar.accessToken);
                    SaveRefreshToken(ar.refreshToken);
                    onSuccess?.Invoke();
                }
                else
                {
                    onError?.Invoke(0, "토큰 재발급 응답 형식이 잘못되었습니다.");
                }
            },
            onError: (code, msg) =>
            {
                ClearJwt();
                ClearRefreshToken();
                Debug.LogError($"[Auth] Token Refresh Failed: {msg}. Clearing tokens.");
                onError?.Invoke(code, msg);
            }
        ));
    }

public void GetUserInfo(Action<UserDataDto> onSuccess, Action<long, string> onError)
{
    StartCoroutine(CoGetAuthorized(GET_USER_INFO_PATH,
        onSuccess: (txt) =>
        {
            try
            {
                Debug.Log($"[User Info Response JSON]: {txt}");
                
                GetUserInfoWrapper wrapper = JsonUtility.FromJson<GetUserInfoWrapper>(txt);
                
                if(wrapper != null && !wrapper.error && wrapper.data != null)
                {
                    var resp = wrapper.data;
                    
                    if(resp != null && !string.IsNullOrEmpty(resp.name))
                    {
                        Debug.Log($"[API] User Info Success: {resp.name}");
                        onSuccess?.Invoke(resp);
                    }
                    else
                    {
                        onError?.Invoke(0, "유저 정보 응답 형식이 잘못되었습니다. (Name field missing)");
                    }
                }
                else if (wrapper != null && wrapper.error)
                {
                    onError?.Invoke(401, wrapper.message); 
                }
                else
                {
                    onError?.Invoke(0, "유저 정보 응답 형식이 잘못되었습니다."); 
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[API] User Info JSON Parse Error: {e.Message}. Response: {txt}");
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

    public void UpdateItem(int itemIndex, int delta, Action<string> onSuccess = null, Action<long, string> onError = null)
    {
        var payload = new ItemUpdateDto { item1 = 0, item2 = 0, item3 = 0, item4 = 0};

        switch(itemIndex)
        {
            case 1 : payload.item1 = delta; break;
            case 2: payload.item2 = delta; break;
            case 3: payload.item3 = delta; break;
            case 4: payload.item4 = delta; break;
            default: 
                Debug.LogError("Invalid item index for update."); 
                onError?.Invoke(0, "Invalid Item Index");
                return;
        }

        StartCoroutine(CoPostJsonAuthorized(ITEM_UPDATE_PATH, payload, onSuccess, onError));
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

    public void GetGameScore(string countryCode, int currentStageGameId, Action<int> onSuccess, Action<long, string> onError)
    {
        string pathWithQuery = $"{GET_GAME_SCORE_PATH}?country={countryCode}";

        StartCoroutine(CoGetAuthorized(pathWithQuery,
            onSuccess: (txt) =>
            {
                try
                {
                    string wrapperJson = $"{{ \"scores\": {txt} }}";

                    var wrapper = JsonUtility.FromJson<StageScoreListWrapper>(wrapperJson);
                    var allScores = wrapper?.scores;

                    if(allScores != null)
                    {
                        var targetScoreData = System.Array.Find(allScores, s => s.gameId == currentStageGameId);
                        if(targetScoreData != null)
                            onSuccess?.Invoke(targetScoreData.score);
                        else
                            onSuccess?.Invoke(0);
                    }
                    else
                    {
                        onError?.Invoke(0, "유효한 점수 응답 데이터 없음");
                    }
                }
                catch(Exception e)
                {
                    Debug.LogError($"오류 발생 : {e.Message}");
                    onError?.Invoke(0, "점수 파싱 오류");
                }
            },
            onError: onError
        ));
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