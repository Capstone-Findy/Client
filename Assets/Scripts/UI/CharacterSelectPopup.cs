using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 


[Serializable]
public struct CharacterSet
{
    public string id;
    public Sprite profileSprite;
    public Sprite bodySprite;
}

public class CharacterSelectPopup : MonoBehaviour
{
    [Header("Character Data")]
    public CharacterSet[] characters;

    [Header("Targets to Apply")]
    public Image profileTarget;
    public Image bodyTarget;

    [Header("Popup & Buttons")]
    public GameObject panelRoot;
    public GameObject dimPanel;

    [SerializeField] private Button[] optionButtons;
    public Button btnConfirm;
    public Button btnClose;

    private int currentIndex;
    private int pendingIndex;
    private const string KEY = "SelectedCharacterIndex";

    void Start()
    {
        currentIndex = PlayerPrefs.GetInt(KEY, 0);
        pendingIndex = currentIndex;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int idx = i;
            optionButtons[i].onClick.AddListener(() => OnSelect(idx));
        }

        btnConfirm.onClick.AddListener(ConfirmSelection);
        btnClose.onClick.AddListener(ClosePopup);
        panelRoot.SetActive(false);
        dimPanel.SetActive(false);
    }

    public void OpenPopup()
    {
        if (panelRoot.activeSelf) return;

        var es = EventSystem.current;     
        if (es != null) es.SetSelectedGameObject(null);

        panelRoot.SetActive(true);
        dimPanel.SetActive(true);
    }

    public void ClosePopup()
    {
        panelRoot.SetActive(false);
        dimPanel.SetActive(false);
    }

    private void OnSelect(int idx)
    {
        pendingIndex = idx;
    }

    private void ConfirmSelection()
    {
        currentIndex = pendingIndex;
        ApplyCharacter(currentIndex);
        PlayerPrefs.SetInt(KEY, currentIndex);
        PlayerPrefs.Save();
        ClosePopup();
    }

    private void ApplyCharacter(int idx)
    {
        if (characters.Length == 0) return;
        var set = characters[idx];
        if (profileTarget != null)
            profileTarget.sprite = set.profileSprite;
        if (bodyTarget != null)
            bodyTarget.sprite = set.bodySprite;
    }

    private void SetInitialCharacter(int idx)
    {
        if (characters.Length == 0) return;
        var set = characters[idx];
        if (profileTarget != null)
            profileTarget.sprite = set.profileSprite;
        if (bodyTarget != null)
            bodyTarget.sprite = set.bodySprite;
    }
}
