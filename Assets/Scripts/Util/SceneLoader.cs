using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public string sceneName;

    void Start()
    {
        Button btn = GetComponent<Button>();

        if(btn != null)
        {
            btn.onClick.AddListener(() =>
            {
                GameManager.instance.LoadScene(sceneName);
            });
        }
    }
}
