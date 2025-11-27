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
    public Button settingButton;
    public Button exitButton;
    public GameObject settingPanel;
    public GameObject[] objects;

    [Header("Volume")]
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Toggle onToggle;
    public Toggle offToggle;

    void OnEnable()
    {
        UpdateUserInfoUI();

        bool isMute = PlayerPrefs.GetInt("MasterMute", 0) == 1;
        if(isMute)
        {   
            onToggle.isOn = false;
            offToggle.isOn = true;
        } 
        else
        {
            onToggle.isOn = true;
            offToggle.isOn = false;
        } 

        onToggle.onValueChanged.AddListener((isOn) =>
        {
            if(isOn)
                SoundManager.instance.SetMasterMute(false);
            else
                SoundManager.instance.SetMasterMute(true);
        });

        if (settingButton != null) settingButton.onClick.RemoveAllListeners();
        if (exitButton != null) exitButton.onClick.RemoveAllListeners();

        if (settingButton != null) 
            settingButton.onClick.AddListener(() =>
            {
                settingPanel.SetActive(true);
                SetObjectsActive(false);
            });
        
        if (exitButton != null) 
            exitButton.onClick.AddListener(() =>
            {
                settingPanel.SetActive(false);
                SetObjectsActive(true);
            });

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
    private void SetObjectsActive(bool isActive)
    {
        if(objects != null)
        {
            for(int i = 0; i < objects.Length; i++)
            {
                if(objects[i] != null)
                {
                    objects[i].SetActive(isActive);
                }
            }
        }
    }
    
}
