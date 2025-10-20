using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("Selections")]
    public CountryData selectedCountry;
    public StageData selectedStage;
    [Header("Util")]
    private Stack<string> sceneHistory = new Stack<string>();

    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    //-----Util-----//
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        sceneHistory.Push(SceneManager.GetActiveScene().name);
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
    public void SelectCountry(CountryData country, bool clearStage = true)
    {
        selectedCountry = country;
        if (clearStage) selectedStage = null;
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
