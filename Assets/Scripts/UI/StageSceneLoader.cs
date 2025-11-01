using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSceneLoader : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GameManager.instance.LoadScene("CountrySelectScene");
        });
    }
}

