using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchasePopupUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button closeButton;

    [Header("Settings")]
    private int currentItemIndex;
    private int currentQuantity = 1;
    private const int UNIT_PRICE = 100;

    private System.Action onPurchaseSuccess;

    void Awake()
    {
        plusButton.onClick.AddListener(OnClickPlus);
        minusButton.onClick.AddListener(OnClickMinus);
        purchaseButton.onClick.AddListener(TryPurchase);

        if(closeButton != null)
            closeButton.onClick.AddListener(ClosePopup);
    }

    public void Open(int itemIndex, Sprite icon, string description, System.Action onSuccessCallback)
    {
        currentItemIndex = itemIndex;
        itemIconImage.sprite = icon;
        descText.text = description;
        onPurchaseSuccess = onSuccessCallback;

        currentQuantity = 1;
        UpdateUI();

        gameObject.SetActive(true);
    }

    private void UpdateUI()
    {
        int totalCost = currentQuantity * UNIT_PRICE;
        priceText.text = $"{currentQuantity}개 구매 - {totalCost}골드";
    }

    private void OnClickPlus()
    {
        currentQuantity++;
        UpdateUI();
    }

    private void OnClickMinus()
    {
        if(currentQuantity > 1)
        {
            currentQuantity--;
            UpdateUI();
        }
    }

    private void TryPurchase()
    {
        var userData = GameManager.instance.currentUserData;
        int totalCost = currentQuantity * UNIT_PRICE;
        
        if(userData.money < totalCost)
        {
            // TODO : 돈 부족 팝업 띄우기
            return;
        }
        DataManager.instance.UpdateItem(currentItemIndex, currentQuantity,
            onSuccess: (txt) =>
            {
                userData.money -= totalCost;

                switch (currentItemIndex)
                {
                    case 1: userData.item1 += currentQuantity; break;
                    case 2: userData.item2 += currentQuantity; break;
                    case 3: userData.item3 += currentQuantity; break;
                    case 4: userData.item4 += currentQuantity; break;
                }

                Debug.Log($"아이템 {currentItemIndex}번 {currentQuantity}개 구매 완료!");
                onPurchaseSuccess?.Invoke();
                ClosePopup();
            },
            onError: (code, msg) =>
            {
                Debug.LogError($"구매 실패: {msg}");
            }
        );
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    } 

}
