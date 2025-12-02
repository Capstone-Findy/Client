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
    public Button shopButton;
    public GameObject settingPanel;
    public GameObject shopPanel;
    public GameObject[] objects;
    [Header("Shop")]
    public Button item1Button;
    public Button item2Button;
    public Button item3Button;
    public Button item4Button;
    private const int ITEM_COST = 100;
    private const int ITEM_INDEX_TO_BUY = 1;

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
        if (item1Button != null) item1Button.onClick.AddListener(() => TryPurchaseItem(ITEM_INDEX_TO_BUY));
        if (item2Button != null) item2Button.onClick.AddListener(() => TryPurchaseItem(ITEM_INDEX_TO_BUY));
        if (item3Button != null) item3Button.onClick.AddListener(() => TryPurchaseItem(ITEM_INDEX_TO_BUY));
        if (item4Button != null) item4Button.onClick.AddListener(() => TryPurchaseItem(ITEM_INDEX_TO_BUY));

      
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

    public void TryPurchaseItem(int itemIndex)
    {
        var userData = GameManager.instance.currentUserData;
        
        if(userData.money < ITEM_COST)
        {
            Debug.LogWarning($"구매 실패: 골드 부족! 현재: {userData.money}, 필요: {ITEM_COST}");
            // TODO: 골드 부족 팝업 띄우기
            return;
        }
        DataManager.instance.UpdateItem(itemIndex, 1,
            onSuccess: (txt) =>
            {
                userData.money -= ITEM_COST;
                if(itemIndex == 1) userData.item1 += 1;
                else if (itemIndex == 2) userData.item2 += 1;
                else if (itemIndex == 3) userData.item3 += 1;
                else userData.item4 += 1;

                Debug.Log($"아이템 {itemIndex} 구매 성공! 100 골드 차감 완료.");
                UpdateUserInfoUI();
            },
            onError: (code, msg) =>
            {
                Debug.LogError($"구매 실패: 서버 통신 오류 (아이템 업데이트). {code} {msg}");
            });
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
