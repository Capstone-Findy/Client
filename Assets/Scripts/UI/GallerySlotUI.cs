using Findy.Define;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GallerySlotUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image thumbnailImage;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private Button slotButton;

    public void Setup(CustomStageInfo info, Texture2D tex, UnityAction onClick)
    {
        dateText.text = info.createdAt;

        if(tex != null)
        {
            thumbnailImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            thumbnailImage.preserveAspect = true;
        }

        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(onClick);
    }
}
