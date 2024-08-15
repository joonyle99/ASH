using UnityEngine;

public class ButtonSoundPlayer : MonoBehaviour
{
    public void PlayHoverSound()
    {
        SoundManager.Instance.PlayCommonSFX("SE_UI_Select");
    }
    public void PlayClickSound()
    {
        SoundManager.Instance.PlayCommonSFX("SE_UI_Button");
    }
}
