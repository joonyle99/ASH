using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : HappyTools.SingletonBehaviourFixed<SoundManager>, ISceneContextBuildListener
{
    [SerializeField] private GameObject _soundListParent;

    [SerializeField] private AudioSource _bgmPlayer;
    [SerializeField] private AudioSource _sfxPlayer;
    [SerializeField] private AudioSource _sfxLoopPlayer;

    [SerializeField] private AudioMixer _audioMixer;

    [Space]

    public AudioSource[] AudioSources;

    private const int PitchPrecision = 1000;

    private SoundList[] _soundLists; // ui, bgm, gimmick ...
    private Dictionary<string, int> _soundListIndexMap = new();
    private Dictionary<int, AudioSource> _pitchedAudioSources1 = new();
    private Dictionary<int, AudioSource> _pitchedAudioSources2 = new();

    protected override void Awake()
    {
        base.Awake();

        _soundLists = _soundListParent.GetComponentsInChildren<SoundList>();

        // each 'sound list'
        for (int i = 0; i < _soundLists.Length; i++)
        {
            // each 'sound data' in sound list
            for (int j = 0; j < _soundLists[i].Datas.Count; j++)
            {
                var soundDataKey = _soundLists[i].Datas[j].Key;
                _soundListIndexMap[soundDataKey] = i;
            }
        }

        _pitchedAudioSources1[PitchPrecision] = _sfxPlayer;
        _pitchedAudioSources2[PitchPrecision] = _sfxLoopPlayer;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            // _pitchedAudioSources1 디버그
            foreach (var audioSource in _pitchedAudioSources1)
            {
                Debug.Log($"_pitchedAudioSources1 => <color=orange>Key</color>: {audioSource.Key}, <color=yellow>Value</color>: {audioSource.Value}", audioSource.Value.gameObject);
            }
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            // _pitchedAudioSources1 디버그
            foreach (var audioSource in _pitchedAudioSources2)
            {
                Debug.Log($"_pitchedAudioSources2 => <color=orange>Key</color>: {audioSource.Key}, <color=yellow>Value</color>: {audioSource.Value}", audioSource.Value.gameObject);
            }
        }
    }

    // volume setting
    public void InitialVolumeSetting()
    {
        float bgmVolume = 1f;
        float sfxVolume = 1f;

        // JsonLoad 예제
        if (JsonDataManager.Has("BGMVolume"))
            bgmVolume = float.Parse(JsonDataManager.Instance.GlobalSaveData.saveDataGroup["BGMVolume"]);

        if (JsonDataManager.Has("SFXVolume"))
            sfxVolume = float.Parse(JsonDataManager.Instance.GlobalSaveData.saveDataGroup["SFXVolume"]);

        SetBgmVolume(bgmVolume);
        SetSfxVolume(sfxVolume);
    }
    public void SetSfxVolume(float volume)
    {
        if (volume < 0.01f)
        {
            _audioMixer.SetFloat("SFX", -80);
        }
        else
        {
            _audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        }

        // JsonSave 예제
        JsonDataManager.Add("SFXVolume", volume.ToString());
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

    // play sound
    public void PlaySFX(SoundClipData soundData, float pitchMultiplier = 1f, float volumeMultiplier = 1f, bool isLoop = false)
    {
        PlaySFX(soundData.Clip, soundData.Pitch * pitchMultiplier, soundData.Volume * volumeMultiplier, isLoop);
    }
    public void PlaySFX(AudioClip clip, float pitchFactor, float volumeFactor, bool isLoop = false)
    {
        if (pitchFactor < 0)
            pitchFactor = 0.001f;

        int pitchKey = Mathf.RoundToInt(pitchFactor * PitchPrecision);

        if (isLoop)
        {
            if (_pitchedAudioSources2.ContainsKey(pitchKey) == false)
            {
                Debug.Log($"_sfxLoopPlayer => AudioSource AddComponent");
                _pitchedAudioSources2[pitchKey] = _sfxLoopPlayer.AddComponent<AudioSource>();
                _pitchedAudioSources2[pitchKey].pitch = pitchFactor;
            }

            // TEMP
            if (clip == _sfxLoopPlayer.clip)
            {
                Debug.Log($"Already Playing this Audio Clip" +
                                 $"\n" +
                                 $"New Audio Source: {clip.name}" +
                                 $"\n" +
                                 $"Old Audio Source: {_sfxLoopPlayer.clip}");
                return;
            }

            _pitchedAudioSources2[pitchKey].Stop();
            _pitchedAudioSources2[pitchKey].clip = clip;
            _pitchedAudioSources2[pitchKey].volume = volumeFactor;
            _pitchedAudioSources2[pitchKey].Play();
        }
        else
        {
            if (_pitchedAudioSources1.ContainsKey(pitchKey) == false)
            {
                Debug.Log($"_sfxPlayer => AudioSource AddComponent (pitchFactor: {pitchFactor})");
                _pitchedAudioSources1[pitchKey] = _sfxPlayer.AddComponent<AudioSource>();
                _pitchedAudioSources1[pitchKey].pitch = pitchFactor;
            }

            _pitchedAudioSources1[pitchKey].PlayOneShot(clip, volumeFactor);
        }
    }
    public void PlayBGM(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip == _bgmPlayer.clip)
        {
            Debug.Log($"Already Playing this Audio Clip" +
                             $"\n" +
                             $"New Audio Source: {clip.name}" +
                             $"\n" +
                             $"Old Audio Source: {_bgmPlayer.clip}");
            return;
        }

        _bgmPlayer.Stop();
        _bgmPlayer.clip = clip;
        _bgmPlayer.volume = volumeMultiplier;
        _bgmPlayer.Play();
    }

    // play common sound (searching in sound list which sound manager has)
    public void PlayCommonSFX(string key, float pitchMultiplier = 1f, float volumeMultiplier = 1f)
    {
        if (_soundListIndexMap.ContainsKey(key))
        {
            _soundLists[_soundListIndexMap[key]].PlaySFX(key, pitchMultiplier, volumeMultiplier);
        }
        else
        {
            Debug.LogWarning("No SFX matching: " + key);
        }
    }
    public void PlayCommonBGM(string key, float volumeMultiplier = 1f)
    {
        if (_soundListIndexMap.TryGetValue(key, out var soundListIndex))
        {
            _soundLists[soundListIndex].PlayBGM(key, volumeMultiplier);
            return;
        }

        Debug.LogWarning("No BGM matching: " + key);
    }
    public void PlayCommonBGMForScene(string sceneName)
    {
        if (GameSceneManager.IsOpeningScene(sceneName))
        {
            PlayCommonBGM("MainTheme");
        }
        else if (GameSceneManager.IsExploration1(sceneName))
        {
            PlayCommonBGM("Exploration1");
        }
        else if (GameSceneManager.IsExploration2(sceneName))
        {
            PlayCommonBGM("Exploration2");
        }
        else if (GameSceneManager.IsBossDungeon1(sceneName)
            || GameSceneManager.IsBossDungeon2(sceneName)
            || GameSceneManager.IsBossScene(sceneName))
        {
            PlayCommonBGM("BossDungeon1");
        }
        else
        {
            // 기본 BGM 재생
            PlayCommonBGM("BasicBGM");
        }
    }

    // stop sound
    public void StopBGM()
    {
        _bgmPlayer.Stop();
    }
    public void StopLoopSFX()
    {
        foreach (var audioSource in _pitchedAudioSources2.Values)
        {
            audioSource.Stop();
        }
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

    // Pause & UnPause All Sound (TODO: Saved AudioSources)
    public void PauseAllSound()
    {
        AudioSources = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (var audioSource in AudioSources)
        {
            audioSource.Pause();
        }
    }
    public void UnPauseAllSound()
    {
        foreach (var audioSource in AudioSources)
        {
            audioSource.UnPause();
        }

        AudioSources = null;
    }

    public void OnSceneContextBuilt()
    {
        AudioSources = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
    }
}
