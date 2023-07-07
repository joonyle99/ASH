using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : HappyTools.SingletonBehaviour<SoundManager>
{
    //Dictionary<float, AudioSource> _audioSources;

    [SerializeField] AudioSource _sfxPlayer;
    [SerializeField] AudioSource _bgmPlayer;

    public void PlaySFXPitched(AudioClip clip, float pitchMultiplier = 1, float volumeMultiplier = 1f)
    {
        _sfxPlayer.PlayOneShot(clip, volumeMultiplier);
    }
    public void PlayBGM(AudioClip clip, float volumeMultiplier = 1f)
    {
        _bgmPlayer.PlayOneShot(clip, volumeMultiplier);
    }
}
