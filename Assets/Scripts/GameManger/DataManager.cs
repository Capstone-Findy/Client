using System.Threading.Tasks;
using UnityEngine;

public static class DataManager
{
    private const string CLEAR_TIME_KEY = "ClearTime";
    private const string UNLOCK_KEY_PREFIX = "UnlockedStageIndex_";
    private const string UNLOCK_COUNTRY_KEY = "UnlockedCountryIndex";


    // ----- Clear Time in Game -----//
    public static bool HasClearTime()
    {
        return PlayerPrefs.HasKey(CLEAR_TIME_KEY);
    }

    public static float GetClearTime()
    {
        return PlayerPrefs.GetFloat(CLEAR_TIME_KEY);
    }

    public static void SaveClearTime(float time)
    {
        PlayerPrefs.SetFloat(CLEAR_TIME_KEY, time);
        PlayerPrefs.Save();
    }

    public static async Task UploadClearTimeToServer(string userId, float time)
    {
        // TODO : 서버에 업로드
    }

    public static async Task<float?> LoadClearTimeFromServer(string userID)
    {
        // TODO : 서버에서 클리어 시간 받아오기
        return null;
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