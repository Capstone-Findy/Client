using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelectUI : MonoBehaviour
{
    [SerializeField] private Button[] stageButtons;
    [SerializeField] private RectTransform layoutArea;
    void Start()
    {
        if (!layoutArea) layoutArea = transform as RectTransform;
        var country = GameManager.instance.selectedCountry;
        int count = Mathf.Min(stageButtons.Length, country.stagesSlots.Count);
        for (int i = 0; i < count; i++)
        {
            var slot = country.stagesSlots[i];
            var btn = stageButtons[i];
            var rt = btn.GetComponent<RectTransform>();

            WireStageButton(btn, slot.stage, i);
            ApplyLayout(rt, slot);
        }
    }

    void WireStageButton(Button btn, StageData stage, int index)
    {
        Image thumb = btn.GetComponentInChildren<Image>(true);
        TextMeshProUGUI title = btn.GetComponentInChildren<TextMeshProUGUI>();

        thumb.sprite = stage.thumbnail;
        title.text = $"Stage {index + 1}";

        btn.onClick.RemoveAllListeners();
        if (stage != null)
        {
            btn.onClick.AddListener(() =>
            {
                GameManager.instance.SelectStage(stage);
                SceneManager.LoadScene("GameScene");
            });
            btn.interactable = true;
        }
        else btn.interactable = false;
    }

    void ApplyLayout(RectTransform rt, StageSlot slot)
    {
        Vector2 areaSize = layoutArea.rect.size;
        Vector2 pivot = layoutArea.pivot;
        Vector2 posPx = new Vector2(slot.normlizedPos.x * areaSize.x, slot.normlizedPos.y * areaSize.y);
        Vector2 anchored = posPx - (areaSize * pivot);

        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchored;
    }
}
