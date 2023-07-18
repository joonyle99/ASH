using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : HappyTools.SingletonBehaviour<SoundManager>
{
    //Dictionary<float, AudioSource> _audioSources;

    [SerializeField] GameObject _soundListParent;

    [SerializeField] AudioSource _sfxPlayer;
    [SerializeField] AudioSource _bgmPlayer;

    SoundList [] _soundLists;
    Dictionary<string, int> _soundListIndicies  = new Dictionary<string, int>();
    protected override void Awake()
    {
        base.Awake();

        _soundLists = _soundListParent.GetComponentsInChildren<SoundList>();
        for(int i = 0; i< _soundLists.Length; i++)
        {
            for (int j = 0; j < _soundLists[i].Datas.Count; j++)
                _soundListIndicies[_soundLists[i].Datas[j].Key] = i;
        }
    }
    public void PlaySFXPitched(AudioClip clip, float pitchMultiplier = 1, float volumeMultiplier = 1f)
    {
        _sfxPlayer.PlayOneShot(clip, volumeMultiplier);
    }
    public void PlayBGM(AudioClip clip, float volumeMultiplier = 1f)
    {
        _bgmPlayer.PlayOneShot(clip, volumeMultiplier);
    }

    public void PlayCommonSFXPitched(string key, float pitchMultiplier = 1, float volumeMultiplier = 1f)
    {
        if (_soundListIndicies.ContainsKey(key))
        {
            _soundLists[_soundListIndicies[key]].PlaySFXPitched(key, pitchMultiplier, volumeMultiplier);
        }
        else
        {
            Debug.LogWarning("No SFX matching: " + key);
        }
    }
    public void PlayCommonBGM(string key, float volumeMultiplier = 1f)
    {
        if (_soundListIndicies.ContainsKey(key))
        {
            _soundLists[_soundListIndicies[key]].PlaySFXPitched(key, volumeMultiplier);
        }
        else
        {
            Debug.LogWarning("No BGM matching: " + key);
        }
    }

}
