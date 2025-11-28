using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountrySelectUI : MonoBehaviour
{
    [SerializeField] private Button[] countryButtons;
    [Header("User Info")]
    public TextMeshProUGUI heartText;
    public TextMeshProUGUI moneyText;
    void Start()
    {
        UpdateUserInfo();
        int unlockedCountryIndex = DataManager.GetUnlockedCountryIndex();

        for (int i = 0; i < countryButtons.Length; i++)
        {
            Button btn = countryButtons[i];

            Transform lockImage = btn.transform.Find("LockImage");
            bool isUnlocked = (i <= unlockedCountryIndex);

            if (isUnlocked)
            {
                btn.interactable = true;
                if (lockImage != null) lockImage.gameObject.SetActive(false);
            }
            else
            {
                btn.interactable = false;
                if (lockImage != null) lockImage.gameObject.SetActive(true);
            }
        }
    }
    
    private void UpdateUserInfo()
    {
        if(GameManager.instance != null && GameManager.instance.currentUserData != null)
        {
            var data = GameManager.instance.currentUserData;
            if(heartText != null) heartText.text = $"{data.heart} / 5";
            if(moneyText != null) moneyText.text = $"{data.money}";
        }
        else
        {
            if(heartText != null) heartText.text = "- / 5";
            if(moneyText != null) moneyText.text = "-";
        }
    }
}
