using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : HappyTools.SingletonBehaviourFixed<SoundManager>
{
    [SerializeField] private GameObject _soundListParent;

    [SerializeField] private AudioSource _sfxPlayer;
    [SerializeField] private AudioSource _bgmPlayer;

    [SerializeField] private AudioMixer _audioMixer;

    private const int PitchPrecision = 1000;

    private SoundList[] _soundLists;
    private Dictionary<string, int> _soundListIndexMap = new Dictionary<string, int>();
    private Dictionary<int, AudioSource> _pitchedAudioSources = new Dictionary<int, AudioSource>();

    protected override void Awake()
    {
        base.Awake();

        _soundLists = _soundListParent.GetComponentsInChildren<SoundList>();

        for (int i = 0; i < _soundLists.Length; i++)
        {
            for (int j = 0; j < _soundLists[i].Datas.Count; j++)
            {
                _soundListIndexMap[_soundLists[i].Datas[j].Key] = i;
            }
        }

        _pitchedAudioSources[1 * PitchPrecision] = _sfxPlayer;
    }
    protected void Start()
    {
        InitialVolumeSetting();
    }

    public void PlaySFXPitched(AudioClip clip, float pitchMultiplier = 1f, float volumeMultiplier = 1f)
    {
        if (pitchMultiplier < 0)
            pitchMultiplier = 0.001f;
        int pitch = Mathf.RoundToInt(pitchMultiplier * PitchPrecision);
        if (!_pitchedAudioSources.ContainsKey(pitch))
        {
            _pitchedAudioSources[pitch] = _sfxPlayer.AddComponent<AudioSource>();
            _pitchedAudioSources[pitch].pitch = (float)pitch / PitchPrecision;
            //_pitchedAudioSources[pitch].spatialBlend = 1f;
            //_pitchedAudioSources[pitch].minDistance = 5f;
            //_pitchedAudioSources[pitch].maxDistance = 30f;
        }

        _pitchedAudioSources[pitch].PlayOneShot(clip, volumeMultiplier);        // 새로운 사운드 출력
    }
    public void PlaySFX(SoundClipData soundData, float volumeMultiplier = 1f)
    {
        PlaySFXPitched(soundData.Clip, soundData.Pitch, volumeMultiplier);
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
    private IEnumerator BGMFadeOutCoroutine(float duration)
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
        if (_soundListIndexMap.ContainsKey(key))
        {
            _soundLists[_soundListIndexMap[key]].PlaySFXPitched(key, pitchMultiplier, volumeMultiplier);
        }
        else
        {
            Debug.LogWarning("No SFX matching: " + key);
        }
    }
    public void PlayCommonBGM(string key, float volumeMultiplier = 1f)
    {
        if (_soundListIndexMap.ContainsKey(key))
        {
            _soundLists[_soundListIndexMap[key]].PlayBGM(key, volumeMultiplier);
        }
        else
        {
            Debug.LogWarning("No BGM matching: " + key);
        }
    }

    public void SetBgmVolume(float volume)
    {
        if (volume < 0.01f)
        {
            _audioMixer.SetFloat("BGM", -80);
        }
        else
        {
            _audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        }

        JsonDataManager.Add("BGMVolume", volume.ToString());
    }
    public void SetSfxVolume(float volume)
    {
        if (volume == 0)
        {
            _audioMixer.SetFloat("SFX", -80);
        }
        else
        {
            _audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        }

        JsonDataManager.Add("SFXVolume", volume.ToString());
    }

    public void InitialVolumeSetting()
    {
        float bgmVolume = 1f;
        float sfxVolume = 1f;

        if (JsonDataManager.Has("BGMVolume"))
            bgmVolume = float.Parse(JsonDataManager._globalSaveData.saveDataGroup["BGMVolume"]);

        if (JsonDataManager.Has("SFXVolume"))
            sfxVolume = float.Parse(JsonDataManager._globalSaveData.saveDataGroup["SFXVolume"]);

        SetBgmVolume(bgmVolume);
        SetSfxVolume(sfxVolume);
    }
}
