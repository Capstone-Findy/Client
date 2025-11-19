using Findy.Define;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    [Header("Sound")]
    public SoundType soundToPlay = SoundType.SFX_Click;

    void Start ()
    {
        Button btn = GetComponent<Button>();
        if(btn != null)
        {
            btn.onClick.AddListener(() => PlaySound()) ;
        }
    }
    void PlaySound()
    {
        if(SoundManager.instance != null)
        {
            SoundManager.instance.PlaySFX(soundToPlay);
        }
    }
}
