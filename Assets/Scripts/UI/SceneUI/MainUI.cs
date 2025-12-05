using Findy.Define;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [Header("User Info Data")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI shopMoneyText;
    public TextMeshProUGUI heartText;
    public TextMeshProUGUI[] itemTexts = new TextMeshProUGUI[4];

    [Header("Shop Data")]
    public PurchasePopupUI purchasePopup;
    public Sprite[] itemIcons;
    [TextArea] public string[] itemDescriptions;

    [Header("UI")]
    public Button settingButton;
    public Button shopButton;
    public GameObject settingPanel;
    public GameObject shopPanel;
    public GameObject[] objects;
    [Header("Shop")]
    public Button item1Button;
    public Button item2Button;
    public Button item3Button;
    public Button item4Button;

    void OnEnable()
    {
        UpdateUserInfoUI();

        if (settingButton != null) settingButton.onClick.RemoveAllListeners();
        if(shopButton != null) shopButton.onClick.RemoveAllListeners();

        if (settingButton != null) 
            settingButton.onClick.AddListener(() =>
            {
                settingPanel.SetActive(true);
                SetObjectsActive(false);
            });

        if(shopButton != null)
        {
            shopButton.onClick.AddListener(() =>
            {
                shopPanel.SetActive(true);
                SetObjectsActive(false);
            });
        }
        if (item1Button != null) item1Button.onClick.AddListener(() => OpenPurchasePopup(1));
        if (item2Button != null) item2Button.onClick.AddListener(() => OpenPurchasePopup(2));
        if (item3Button != null) item3Button.onClick.AddListener(() => OpenPurchasePopup(3));
        if (item4Button != null) item4Button.onClick.AddListener(() => OpenPurchasePopup(4));

      
    }

    private void UpdateUserInfoUI()
    {
        if(GameManager.instance == null) return;

        UserDataDto userData = GameManager.instance.currentUserData;
        if(userData == null) return;

        if(nameText != null) nameText.text = userData.name;
        if(moneyText != null) moneyText.text = $"{userData.money}";
        if(shopMoneyText != null) shopMoneyText.text = $"{userData.money}";
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
    private void OpenPurchasePopup(int itemIndex)
    {
        int arrayIndex = itemIndex - 1;

        if (arrayIndex < 0 || arrayIndex >= itemIcons.Length) return;

        Sprite icon = itemIcons[arrayIndex];
        string desc = itemDescriptions[arrayIndex];

        purchasePopup.Open(itemIndex, icon, desc, UpdateUserInfoUI);
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
    public void CloseAllPanel()
    {
        settingPanel.SetActive(false);
        shopPanel.SetActive(false);
        SetObjectsActive(true);
    }
    
}
