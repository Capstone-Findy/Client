using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountrySelectUI : MonoBehaviour
{
    [SerializeField] private Button[] countryButtons;
    void Awake()
    {
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

                // TODO : StageSelectScene으로 이동하는 로직 병합(CountryButtonHook)
            }
            else
            {
                btn.interactable = false;
                if (lockImage != null) lockImage.gameObject.SetActive(true);
            }
        }
    }
}
