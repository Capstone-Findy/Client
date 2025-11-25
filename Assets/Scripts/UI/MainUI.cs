using Findy.Define;
using TMPro;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    [Header("User Info Data")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI heartText;
    public TextMeshProUGUI[] itemTexts = new TextMeshProUGUI[4];

    void OnEnable()
    {
        UpdateUserInfoUI();
    }

    private void UpdateUserInfoUI()
    {
        if(GameManager.instance == null) return;

        UserDataDto userData = GameManager.instance.currentUserData;
        if(userData == null) return;

        if(nameText != null) nameText.text = userData.name;
        if(moneyText != null) moneyText.text = $"{userData.money}";
        if(heartText != null) heartText.text = $"5 / {userData.heart}";

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
