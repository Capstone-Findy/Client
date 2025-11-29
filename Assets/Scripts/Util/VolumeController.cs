using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [Header("Volume")]
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Toggle onToggle;
    public Toggle offToggle;

    void OnEnable()
    {
        bool isMute = PlayerPrefs.GetInt("MasterMute", 0) == 1;
        if(isMute)
        {   
            onToggle.isOn = false;
            offToggle.isOn = true;
        } 
        else
        {
            onToggle.isOn = true;
            offToggle.isOn = false;
        } 

        onToggle.onValueChanged.AddListener((isOn) =>
        {
            if(isOn)
                SoundManager.instance.SetMasterMute(false);
            else
                SoundManager.instance.SetMasterMute(true);
        });

        if (bgmSlider != null)
        {
            bgmSlider.value = PlayerPrefs.GetFloat("BGM_Volume", 1f);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFX_Volume", 1f);
        }
    }
}
