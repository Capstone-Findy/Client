using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpdatePopup : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    void Start()
    {
        if(closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Close);
        }
    }

    public void Show()
    {
        gameObject.SetActive(false);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
