using System.Collections;
using System.Collections.Generic;
using Findy.Define;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomSelectUI : MonoBehaviour
{
    [Header("UI")]
    public Transform contentRoot;
    public GameObject slotPrefab;
    public GameObject detailPopupPanel;
    public Image detailImage;
    public TextMeshProUGUI detailDate;
    public TextMeshProUGUI detailPrompt;
    public Button playButton;
    public GameObject modeSelectPanel;
    public GameObject helpPanel;
    public Button modeSelectButton;
    public Button helpButton;

    private CustomStageInfo currentSelectedInfo;

    void Start()
    {   
        detailPopupPanel.SetActive(false);
        playButton.onClick.AddListener(OnPlayClicked);
        LoadGallery();

        if(modeSelectButton != null)
            modeSelectButton.onClick.AddListener(() =>
            {
                modeSelectPanel.SetActive(true);
            });
        if(helpButton != null)
            helpButton.onClick.AddListener(() =>
            {
                bool isActive = helpPanel.activeSelf;
               helpPanel.SetActive(!isActive); 
            });
    }

    public void LoadGallery()
    {
        foreach(Transform child in contentRoot)
        {
            Destroy(child.gameObject);
        }
        if(CustomDataHandler.instance == null) return;
        CustomStageList list = CustomDataHandler.instance.LoadAllMeta();
        list.stages.Reverse();

        foreach(var info in list.stages)
        {
            Texture2D tex = CustomDataHandler.instance.LoadTexture(info.id + "_orig.png");
            GameObject obj = Instantiate(slotPrefab, contentRoot);
            GallerySlotUI slotUI = obj.GetComponent<GallerySlotUI>();

            slotUI.Setup(info, tex, () => ShowDetailPopup(info));
        }
    }

    public void ShowDetailPopup(CustomStageInfo info)
    {
        currentSelectedInfo = info;
        
        detailDate.text = info.createdAt;
        detailPrompt.text = info.prompt;

        Texture2D tex = CustomDataHandler.instance.LoadTexture(info.id + "_orig.png");
        if(tex != null)
        {
            detailImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            detailImage.preserveAspect = true;
        }
        detailPopupPanel.SetActive(true);
    }

    public void OnPlayClicked()
    {
        if(currentSelectedInfo == null) return;

        StageData stageData = CustomDataHandler.instance.LoadStageData(currentSelectedInfo);

        if(stageData != null)
        {
            GameManager.instance.returnSceneName = "CustomSelectScene";
            GameManager.instance.SelectStage(stageData);
            GameManager.instance.LoadScene("GameScene");
        }
    }

    public void CloseAllPanels()
    {
        modeSelectPanel.SetActive(false);
        detailPopupPanel.SetActive(false);
        helpPanel.SetActive(false);
    }
}
