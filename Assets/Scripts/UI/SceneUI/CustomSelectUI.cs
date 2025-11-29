using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomSelectUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject modeSelectPanel;
    public GameObject helpPanel;
    public Button modeSelectButton;
    public Button helpButton;
    public Button exitButton;

    void Start()
    {   
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
        if (exitButton != null) 
            exitButton.onClick.AddListener(() =>
            {
                modeSelectPanel.SetActive(false);
                helpPanel.SetActive(false);
            });
    }
}
