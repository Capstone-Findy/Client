using Findy.Define;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [Header("User Info Data")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI heartText;
    public TextMeshProUGUI[] itemTexts = new TextMeshProUGUI[4];

    [Header("UI")]
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Button settingButton;
    public Button exitButton;
    public GameObject settingPanel;


    void OnEnable()
    {
        UpdateUserInfoUI();

        if (settingButton != null) settingButton.onClick.RemoveAllListeners();
        if (exitButton != null) exitButton.onClick.RemoveAllListeners();

        if (settingButton != null) 
            settingButton.onClick.AddListener(() => settingPanel.SetActive(true));
        
        if (exitButton != null) 
            exitButton.onClick.AddListener(() => settingPanel.SetActive(false));

        if (bgmSlider != null)
        {
            bgmSlider.value = PlayerPrefs.GetFloat("BGM_Volume", 1f);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFX_Volume", 1f);
        }
    }

    private void UpdateUserInfoUI()
    {
        if(GameManager.instance == null) return;

        UserDataDto userData = GameManager.instance.currentUserData;
        if(userData == null) return;

        if(nameText != null) nameText.text = userData.name;
        if(moneyText != null) moneyText.text = $"{userData.money}";
        if(heartText != null) heartText.text = $"{userData.heart} / 5";

        if(itemTexts.Length >= 4)
        {
            int[] itemCounts = new int[] {userData.item1, userData.item2, userData.item3, userData.item4};

            for(int i = 0; i < itemCounts.Length; i++)
            {
                if(itemTexts[i] != null)
                {
                    itemTexts[i].text = $"{itemCounts[i]}";
                }
            }
        }
        
    }
}
