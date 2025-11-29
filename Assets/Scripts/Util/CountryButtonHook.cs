using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CountryButtonHook : MonoBehaviour
{
    public CountryData countryData;
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GameManager.instance.SelectCountry(countryData);
            GameManager.instance.LoadScene("StageSelectScene");
        });
    }
}
