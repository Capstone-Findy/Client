using TMPro;
using UnityEngine;

public class HeartHUD : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI heartCountText;
    [SerializeField] private TextMeshProUGUI heartTimerText;

    void Update()
    {
        UpdateHeartUI();
    }
    private void UpdateHeartUI()
    {
        if (GameManager.instance == null || GameManager.instance.currentUserData == null) return;

        var userData = GameManager.instance.currentUserData;
        int maxHeart = GameManager.instance.GetMaxHeart();

        if (heartCountText != null)
        {
            heartCountText.text = $"{userData.heart}";
        }

        if (heartTimerText != null)
        {
            if (userData.heart >= maxHeart)
            {
                heartTimerText.text = "MAX";
            }
            else
            {
                float remainTime = GameManager.instance.GetRemainingRegenTime();

                int minutes = Mathf.FloorToInt(remainTime / 60);
                int seconds = Mathf.FloorToInt(remainTime % 60);

                heartTimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }
    }
}
