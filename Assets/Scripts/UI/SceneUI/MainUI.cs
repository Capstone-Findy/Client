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
    public Button myPageButton;
    public Button logoutButton;
    public Button withdrawButton;
    public GameObject settingPanel;
    public GameObject shopPanel;
    public GameObject myPagePanel;
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
        if (myPageButton != null) myPageButton.onClick.RemoveAllListeners();

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
        if(myPageButton != null)
        {
            myPageButton.onClick.AddListener(() =>
            {
                myPagePanel.SetActive(true);
                SetObjectsActive(false);
            });
        }
        if (logoutButton != null)
        {
            logoutButton.onClick.RemoveAllListeners();
            logoutButton.onClick.AddListener(OnClickLogout);
        }

        if (withdrawButton != null)
        {
            withdrawButton.onClick.RemoveAllListeners();
            withdrawButton.onClick.AddListener(OnClickWithdraw);
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
    private void OnClickLogout()
    {
        int userId = GameManager.instance.currentUserData != null ? GameManager.instance.currentUserData.id : 0;

        DataManager.instance.Logout(userId,
            onSuccess: () =>
            {
                GameManager.instance.LoadScene("LoginScene", false);
            },
            onError: (code, msg) =>
            {
                Debug.LogError("로그아웃 실패");
                GameManager.instance.LoadScene("LoginScene", false);
            }
        );
    }
    private void OnClickWithdraw()
    {      
        DataManager.instance.Withdraw(
            onSuccess: () =>
            {
                Debug.Log("회원탈퇴 완료. 로그인 화면으로 이동합니다.");
                GameManager.instance.LoadScene("LoginScene", false);
            },
            onError: (code, msg) =>
            {
                Debug.LogError($"회원탈퇴 실패: {msg}");
            }
        );
    }
    public void CloseAllPanel()
    {
        settingPanel.SetActive(false);
        shopPanel.SetActive(false);
        myPagePanel.SetActive(false);
        SetObjectsActive(true);
    }
}
