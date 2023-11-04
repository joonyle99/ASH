using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : HappyTools.SingletonBehaviour<SoundManager>
{
    Dictionary<int, AudioSource> _pitchedAudioSources = new Dictionary<int, AudioSource>();

    [SerializeField] GameObject _soundListParent;

    [SerializeField] AudioSource _sfxPlayer;
    [SerializeField] AudioSource _bgmPlayer;

    const int PitchPrecision = 1000;

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
        _pitchedAudioSources[1 * PitchPrecision] = _sfxPlayer;
    }
    public void PlaySFXPitched(AudioClip clip, float pitchMultiplier = 1f, float volumeMultiplier = 1f)
    {
        if (pitchMultiplier < 0)
            pitchMultiplier = 0.001f;
        int pitch = Mathf.RoundToInt(pitchMultiplier * PitchPrecision);
        if (!_pitchedAudioSources.ContainsKey(pitch))
        {
            _pitchedAudioSources[pitch] = _sfxPlayer.AddComponent<AudioSource>();
            _pitchedAudioSources[pitch].pitch = pitch / 1000f;
            //_pitchedAudioSources[pitch].spatialBlend = 1f;
            //_pitchedAudioSources[pitch].minDistance = 5f;
            //_pitchedAudioSources[pitch].maxDistance = 30f;
        }
        _pitchedAudioSources[pitch].Stop();                                     // 이전 사운드 종료
        _pitchedAudioSources[pitch].PlayOneShot(clip, volumeMultiplier);        // 새로운 사운드 출력
    }
    public void PlayBGM(AudioClip clip, float volumeMultiplier = 1f, bool replayIfSameClip = false)
    {
        if (replayIfSameClip && clip == _bgmPlayer.clip)
            return;
        _bgmPlayer.Stop();
        _bgmPlayer.clip = clip;
        _bgmPlayer.volume = volumeMultiplier;
        _bgmPlayer.Play();
    }

    public void StopBGM()
    {
        _bgmPlayer.Stop();
    }
    public void StopBGMFade(float duration)
    {
        StartCoroutine(BGMFadeOutCoroutine(duration));
        _bgmPlayer.Stop();
    }
    IEnumerator BGMFadeOutCoroutine(float duration)
    {
        float eTime = 0f;
        float originalChannelVolume = _bgmPlayer.volume;
        while (eTime < duration)
        {
            eTime += Time.deltaTime;
            yield return null;
            if (!_bgmPlayer.isPlaying)
                yield break;
            float t = eTime / duration;
            _bgmPlayer.volume = Mathf.Lerp(originalChannelVolume, 0, t);
        }
        if (!_bgmPlayer.isPlaying)
            yield break;
        _bgmPlayer.Stop();
    }

    public void PlayCommonSFXPitched(string key, float pitchMultiplier = 1f, float volumeMultiplier = 1f)
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
            _soundLists[_soundListIndicies[key]].PlayBGM(key, volumeMultiplier);
        }
        else
        {
            Debug.LogWarning("No BGM matching: " + key);
        }
    }

}
