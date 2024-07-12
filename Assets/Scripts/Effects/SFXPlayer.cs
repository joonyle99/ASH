using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip _sfxAudioClip;
    [SerializeField] private SoundClipData _sfxSoundData;

    public void PlayAudioClip()
    {
        SoundManager.Instance.PlaySFXPitched(_sfxAudioClip);
    }
    public void PlaySoundClip()
    {
        SoundManager.Instance.PlaySFX(_sfxSoundData, _sfxSoundData.Volume);
    }
}
