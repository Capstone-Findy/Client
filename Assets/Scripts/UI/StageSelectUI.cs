using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelectUI : MonoBehaviour
{
    [SerializeField] private Button[] stageButtons;
    [SerializeField] private Image[] arrowImages;
    [SerializeField] private Image mapImage;
    [SerializeField] private RectTransform layoutArea;

    [Header("Stage Info Panel")]
    [SerializeField] private GameObject stageInfoPanel;
    [SerializeField] private TextMeshProUGUI panelStageTitle;
    [SerializeField] private TextMeshProUGUI panelMissionText;
    [SerializeField] private Image panelStageImage;
    [SerializeField] private TextMeshProUGUI panelStageDescription;
    [SerializeField] private Button panelStartButton;
    [SerializeField] private Button panelCloseButton;
    void Start()
    {
        if (!layoutArea) layoutArea = transform as RectTransform;
        var country = GameManager.instance.selectedCountry;
        int unlockedIndex = DataManager.GetUnlockedStageIndex(country.countryName);
        int count = Mathf.Min(stageButtons.Length, country.stagesSlots.Count);
        if (mapImage != null)
        {
            mapImage.sprite = country.background;
            mapImage.rectTransform.sizeDelta = country.backgroundSize;
        }
        for (int i = 0; i < count; i++)
        {
            var slot = country.stagesSlots[i];
            var btn = stageButtons[i];
            var rt = btn.GetComponent<RectTransform>();

            WireStageButton(btn, slot.stage, i, unlockedIndex);
            ApplyStageLayout(rt, slot);
        }

        int arrowCount = Mathf.Min(arrowImages.Length, GameManager.instance.selectedCountry.arrowSlots.Count);
        for (int i = 0; i < arrowCount; i++)
        {
            var img = arrowImages[i];
            var aslot = GameManager.instance.selectedCountry.arrowSlots[i];
            ApplyArrowLayout(img.rectTransform, aslot);
            img.gameObject.SetActive(false);
        }
        for (int i = arrowCount; i < arrowImages.Length; i++)
        {
            if (arrowImages[i] != null) arrowImages[i].gameObject.SetActive(false);
        }

        if(panelCloseButton != null)
        {
            panelCloseButton.onClick.AddListener(() => stageInfoPanel.SetActive(false));
        }
    }

    void WireStageButton(Button btn, StageData stage, int index, int unlockedIndex)
    {
        Image thumb = btn.GetComponentInChildren<Image>(true);
        TextMeshProUGUI title = btn.GetComponentInChildren<TextMeshProUGUI>();

        Transform lockImage = btn.transform.Find("LockImage");

        thumb.sprite = stage.thumbnail;
        title.text = $"Stage {index + 1}";

        btn.onClick.RemoveAllListeners();

        bool isUnlocked = (index <= unlockedIndex);
        if (isUnlocked && stage != null)
        {
            btn.interactable = true;
            if(lockImage != null)
            {
                lockImage.gameObject.SetActive(false);
            }
            btn.onClick.AddListener(() =>
            {
                GameManager.instance.SelectStage(stage);
                ShowStageInfoPanel(stage, index);
            });
            btn.interactable = true;
        }
        else
        {
            btn.interactable = false;
            if(lockImage != null)
            {
                lockImage.gameObject.SetActive(true);
            }
        }    
    }

    void ShowStageInfoPanel(StageData stage, int index)
    {
        if (stageInfoPanel == null || stage == null) return;

        if (panelStageTitle != null)
            panelStageTitle.text = $"Stage {index + 1}";

        if (panelMissionText != null)
            panelMissionText.text = stage.stageMission;

        if (panelStageImage != null)
            panelStageImage.sprite = stage.originalImage;

        if (panelStageDescription != null)
            panelStageDescription.text = stage.stageDescription;

        if (panelStartButton != null)
        {
            panelStartButton.onClick.RemoveAllListeners();
            panelStartButton.onClick.AddListener(() =>
            {
                // TODO : 데이터 연결 시 추후 제거
                GameManager.instance.LoadScene("GameScene");

                // GameManager.instance.TryStartStage(
                //     onError: (code, msg) =>
                //     {
                //         // TODO : 하트 부족 UI 패널 띄우기
                //     }
                // );
            });
        }
        stageInfoPanel.SetActive(true);
    }

    void ApplyStageLayout(RectTransform rt, StageSlot slot)
    {
        Vector2 areaSize = layoutArea.rect.size;
        Vector2 pivot = layoutArea.pivot;
        Vector2 posPx = new Vector2(slot.StageImagePos.x * areaSize.x, slot.StageImagePos.y * areaSize.y);
        Vector2 anchored = posPx - (areaSize * pivot);

        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchored;
    }

    void ApplyArrowLayout(RectTransform rt, ArrowSlot slot)
    {
        Vector2 areaSize = layoutArea.rect.size;
        Vector2 pivot = layoutArea.pivot;
        Vector2 posPx = new Vector2(slot.ArrowImagePos.x * areaSize.x, slot.ArrowImagePos.y * areaSize.y);
        Vector2 anchored = posPx - (areaSize * pivot);

        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchored;

        rt.localRotation = Mathf.Abs(slot.rotation) > 0.01f ? Quaternion.Euler(0, 0, slot.rotation) : Quaternion.identity;
    }
}
