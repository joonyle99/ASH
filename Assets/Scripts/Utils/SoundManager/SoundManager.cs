using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : HappyTools.SingletonBehaviourFixed<SoundManager>
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

        // each sound list
        for (int soundListIndex = 0; soundListIndex < _soundLists.Length; soundListIndex++)
        {
            // each sound data in sound list
            for (int soundDataIndex = 0; soundDataIndex < _soundLists[soundListIndex].Datas.Count; soundDataIndex++)
            {
                var soundDataKey = _soundLists[soundListIndex].Datas[soundDataIndex].Key;
                _soundListIndexMap[soundDataKey] = soundListIndex;
            }
        }

        _pitchedAudioSources1[1 * PitchPrecision] = _sfxPlayer;
        _pitchedAudioSources2[1 * PitchPrecision] = _sfxLoopPlayer;
    }
    protected void Start()
    {

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
        if (volume == 0)
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
    public void PlaySFX(AudioClip clip, float pitchMultiplier = 1f, float volumeMultiplier = 1f, bool isLoop = false)
    {
        if (pitchMultiplier < 0)
            pitchMultiplier = 0.001f;

        int pitch = Mathf.RoundToInt(pitchMultiplier * PitchPrecision);

        if (isLoop)
        {
            if (!_pitchedAudioSources2.ContainsKey(pitch))
            {
                _pitchedAudioSources2[pitch] = _sfxLoopPlayer.AddComponent<AudioSource>();
                _pitchedAudioSources2[pitch].pitch = (float)pitch / PitchPrecision;
            }

            if (clip == _sfxLoopPlayer.clip)
            {
                Debug.Log($"Already Playing this Audio Clip" +
                                 $"\n" +
                                 $"New Audio Source: {clip.name}" +
                                 $"\n" +
                                 $"Old Audio Source: {_sfxLoopPlayer.clip}");
                return;
            }

            _pitchedAudioSources2[pitch].Stop();
            _pitchedAudioSources2[pitch].clip = clip;
            _pitchedAudioSources2[pitch].volume = volumeMultiplier;
            _pitchedAudioSources2[pitch].Play();
        }
        else
        {
            if (!_pitchedAudioSources1.ContainsKey(pitch))
            {
                _pitchedAudioSources1[pitch] = _sfxPlayer.AddComponent<AudioSource>();
                _pitchedAudioSources1[pitch].pitch = (float)pitch / PitchPrecision;
            }

            _pitchedAudioSources1[pitch].PlayOneShot(clip, volumeMultiplier);
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
}
