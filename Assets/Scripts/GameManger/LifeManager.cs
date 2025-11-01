using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LifeManager : MonoBehaviour
{
    public int maxLife = 5;
    public int currentLife = 3;
    public float recoverInterval = 600f; // 10분 (초 단위)
    private float timer;

    public TextMeshProUGUI lifeText; 

    private const string LastRecoverTimeKey = "LastRecoverTime";
    private const string CurrentLifeKey = "CurrentLife";
    
  

    void Start()
    {
        // 저장된 목숨 불러오기
        currentLife = PlayerPrefs.GetInt(CurrentLifeKey, maxLife);

        // 앱을 껐다 켜도 회복 시간 계산
        string lastTimeString = PlayerPrefs.GetString(LastRecoverTimeKey, "");
        if (!string.IsNullOrEmpty(lastTimeString))
        {
            DateTime lastTime = DateTime.Parse(lastTimeString);
            TimeSpan elapsed = DateTime.Now - lastTime;
            int recovered = Mathf.FloorToInt((float)elapsed.TotalSeconds / recoverInterval);

            if (recovered > 0)
            {
                currentLife = Mathf.Min(currentLife + recovered, maxLife);
            }
        }

        UpdateLifeUI();
    }

    void Update()
    {
        if (currentLife < maxLife)
        {
            timer += Time.deltaTime;
            if (timer >= recoverInterval)
            {
                timer = 0f;
                currentLife++;
                UpdateLifeUI();
                PlayerPrefs.SetInt(CurrentLifeKey, currentLife);
                PlayerPrefs.SetString(LastRecoverTimeKey, DateTime.Now.ToString());
            }
        }
    }

    public void UseLife(int amount = 1)
    {
        if (currentLife >= amount)
        {
            currentLife -= amount;
            UpdateLifeUI();
            PlayerPrefs.SetInt(CurrentLifeKey, currentLife);
            PlayerPrefs.SetString(LastRecoverTimeKey, DateTime.Now.ToString());
        }
    }

    private void UpdateLifeUI()
    {
        lifeText.text = $"{currentLife} / {maxLife}";
    }
}
