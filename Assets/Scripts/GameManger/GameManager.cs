using System;
using System.Collections;
using System.Collections.Generic;
using Findy.Define;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("User Data")]
    public UserDataDto currentUserData;
    [Header("Heart System")]
    private const int MAX_HEART = 5;
    private const int REGEN_TIME = 600;
    private const string EXIT_TIME_KEY = "Heart_ExitTime";
    private float heartTimer = 0f;
    [Header("Selections")]
    public CountryData selectedCountry;
    public StageData selectedStage;
    [Header("Game Data")]
    [SerializeField] private List<CountryData> allcountries = new List<CountryData>();
    
    [Header("Util")]
    private Stack<string> sceneHistory = new Stack<string>();

    [Header("Navigation")]
    public string returnSceneName = "CustomMakeScene2";

    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);

        Application.targetFrameRate = 60;
    }

    void Start()
    {
        if(currentUserData != null)
        {
            CheckOfflineHeartRegen();
        }
    }

    void Update()
    {
        if(currentUserData != null)
        {
            if(currentUserData.heart < MAX_HEART)
            {
                heartTimer += Time.deltaTime;
                if(heartTimer >= REGEN_TIME)
                {
                    heartTimer -= REGEN_TIME;
                    AddHeart(1);
                    SaveExitTime();
                }
            }
            else
            {
                heartTimer = 0f;
            }
        }
    }
    //----- Heart System -----//
    void OnApplicationPause(bool pauseStatus)
    {
        if(pauseStatus)
        {
            SaveExitTime();
        }
        else
        {
            CheckOfflineHeartRegen();
        }
    }
    void OnApplicationQuit()
    {
        SaveExitTime();
    }
    private void SaveExitTime()
    {
        PlayerPrefs.SetString(EXIT_TIME_KEY, DateTime.Now.ToBinary().ToString());
        PlayerPrefs.Save();
    }
    private void CheckOfflineHeartRegen()
    {
        if(currentUserData == null || currentUserData.heart >= MAX_HEART) return;
        if(!PlayerPrefs.HasKey(EXIT_TIME_KEY)) return;

        string timeStr = PlayerPrefs.GetString(EXIT_TIME_KEY);
        long temp = Convert.ToInt64(timeStr);
        DateTime lastExitTime = DateTime.FromBinary(temp);

        TimeSpan timePassed = DateTime.Now - lastExitTime;
        double secondsPassed = timePassed.TotalSeconds;

        if(secondsPassed >= REGEN_TIME)
        {
            int heartsToGain = (int)(secondsPassed / REGEN_TIME);
            float remainingSeconds = (float)(secondsPassed % REGEN_TIME);

            int spaceLeft = MAX_HEART - currentUserData.heart;
            int finalGain = Mathf.Min(heartsToGain, spaceLeft);

            if(finalGain > 0)
            {
                AddHeart(finalGain);
            }
            if(currentUserData.heart < MAX_HEART)
            {
                heartTimer = remainingSeconds;
            }
        }
        else
        {
            heartTimer += (float)secondsPassed;
        }
    }

    public void AddHeart(int count)
    {
        if(currentUserData.heart >= MAX_HEART) return;

        int prevHeart = currentUserData.heart;
        currentUserData.heart = Mathf.Min(currentUserData.heart + count, MAX_HEART);

        int actualGain = currentUserData.heart - prevHeart;

        if(actualGain > 0)
        {
            DataManager.instance.UpdateHeart(actualGain);
        }
    }

    //----- Sound -----//
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainScene":
            case "LoginScene":
            case "StartScene":
                SoundManager.instance.PlayBGM(SoundType.BGM_Main1);
                break;
            case "CountrySelectScene":
            case "StageSelectScene":
            case "CustomSelectScene":
            case "CustomMakeScene1":
            case "CustomMakeScene2":
                SoundManager.instance.PlayBGM(SoundType.BGM_Main2);
                break;
            case "GameScene":
                SoundManager.instance.PlayBGM(SoundType.BGM_InGame);
                break;
        }
    }
    //-----Util-----//
    public void PopSceneHistory()
    {
        if(sceneHistory.Count > 0)
        {
            sceneHistory.Pop();
        }
    }
    public void LoadScene(string sceneName, bool recordHistory = true)
    {
        Time.timeScale = 1f;
        if(recordHistory)
        {
            sceneHistory.Push(SceneManager.GetActiveScene().name);
        }
        SceneManager.LoadScene(sceneName);
    }
    public void GoBack()
    {
        if (sceneHistory.Count > 0)
        {
            Time.timeScale = 1f;
            string previousScene = sceneHistory.Pop();
            SceneManager.LoadScene(previousScene);
        }
        else Application.Quit();
    }

    //-----Country Select Scene-----//
    public int GetCountryIndex(CountryData country)
    {
        return allcountries.FindIndex(c => c == country);
    }
    public int GetCountryCount()
    {
        return allcountries.Count;
    }
    public void SelectCountry(CountryData country, bool clearStage = true)
    {
        selectedCountry = country;
        if (clearStage) selectedStage = null;
    }

    public void TryStartStage(Action<long, string> onError)
    {
        if(currentUserData == null || currentUserData.heart <= 0) return;

        DataManager.instance.UpdateHeart(-1,
            onSuccess: () =>
            {
                currentUserData.heart--;
                LoadScene("GameScene");
            },
            onError: (code, msg) =>
            {
                onError?.Invoke(code, msg);
            }
        );
    }

    public void SelectStage(StageData stage)
    {
        selectedStage = stage;
    }

    public void ClearSelection()
    {
        selectedCountry = null;
        selectedStage = null;
    }
}
