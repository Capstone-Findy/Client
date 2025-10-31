using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSceneLoader : MonoBehaviour
{
    public void LoadBasicMode()
    {
        SceneManager.LoadScene("CountrySelectScene");
    }
}

