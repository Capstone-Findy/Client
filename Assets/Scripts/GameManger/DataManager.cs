using System.Threading.Tasks;
using UnityEngine;

public static class DataManager
{
    private const string CLEAR_TIME_KEY = "ClearTime";

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
}