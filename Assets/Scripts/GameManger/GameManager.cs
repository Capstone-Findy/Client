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
    [Header("Selections")]
    public CountryData selectedCountry;
    public StageData selectedStage;
    [Header("Game Data")]
    [SerializeField] private List<CountryData> allcountries = new List<CountryData>();
    [Header("Util")]
    private Stack<string> sceneHistory = new Stack<string>();

    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
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
                SoundManager.instance.PlayBGM(SoundType.BGM_Main1);
                break;
            case "CountrySelectScene":
            case "StageSelectScene":
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
