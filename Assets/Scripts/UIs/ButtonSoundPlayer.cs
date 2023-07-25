using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSoundPlayer : MonoBehaviour
{
    public void PlayHoverSound()
    {
        SoundManager.Instance.PlayCommonSFXPitched("SE_UI_Select");
    }
    public void PlayClickSound()
    {
        SoundManager.Instance.PlayCommonSFXPitched("SE_UI_Button");
    }
}
