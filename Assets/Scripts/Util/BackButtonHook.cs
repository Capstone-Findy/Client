using UnityEngine;
using UnityEngine.UI;

public class BackButtonHook : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.GoBack();
            }    
        });
    }
}
