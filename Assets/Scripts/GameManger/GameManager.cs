using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("Selections")]
    public CountryData selectedCountry;
    public StageData selectedStage;

    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
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
