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

    public List<AudioSource> allSFXPlayer;

    private const int PitchPrecision = 1000;

    private SoundList[] _soundLists; // ui, bgm, gimmick ...
    private Dictionary<string, int> _soundListIndexMap = new();
    private Dictionary<int, AudioSource> _pitchedSFXPlayer = new();
    private Dictionary<int, AudioSource> _pitchedSFXLoopPlayer = new();

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

        _pitchedSFXPlayer[PitchPrecision] = _sfxPlayer;
        _pitchedSFXLoopPlayer[PitchPrecision] = _sfxLoopPlayer;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (var sfxPlayer in _pitchedSFXPlayer)
            {
                Debug.Log($"_pitchedAudioSources1 => <color=orange>Key</color>: {sfxPlayer.Key}, <color=yellow>Value</color>: {sfxPlayer.Value}", sfxPlayer.Value.gameObject);
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            foreach (var sfxPlayer in _pitchedSFXLoopPlayer)
            {
                Debug.Log($"_pitchedAudioSources2 => <color=orange>Key</color>: {sfxPlayer.Key}, <color=yellow>Value</color>: {sfxPlayer.Value}", sfxPlayer.Value.gameObject);
            }
        }
#endif
    }

    // volume setting
    public void InitialVolumeSetting()
    {
        // Debug.Log("InitialVolumeSetting");

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
        // Debug.Log("SetSfxVolume");

        if (volume < 0.01f)
        {
            _audioMixer.SetFloat("SFX", -80);

            for (int i = 0; i < allSFXPlayer.Count; i++)
            {
                if (allSFXPlayer[i] == null)
                {
                    allSFXPlayer.RemoveAt(i);
                    continue;
                }

                allSFXPlayer[i].mute = true;
                allSFXPlayer[i].volume = volume;
            }
        }
        else
        {
            _audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);

            for (int i = 0; i < allSFXPlayer.Count; i++)
            {
                if (allSFXPlayer[i] == null)
                {
                    allSFXPlayer.RemoveAt(i);
                    continue;
                }

                allSFXPlayer[i].mute = false;
                allSFXPlayer[i].volume = volume;
            }
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
            if (_pitchedSFXLoopPlayer.ContainsKey(pitchKey) == false)
            {
                // Debug.Log($"_sfxLoopPlayer => AudioSource AddComponent (pitchFactor: {pitchFactor})");

                _pitchedSFXLoopPlayer[pitchKey] = _sfxLoopPlayer.AddComponent<AudioSource>();
                _pitchedSFXLoopPlayer[pitchKey].pitch = pitchFactor;

                if (allSFXPlayer.Contains(_pitchedSFXLoopPlayer[pitchKey]) == false)
                    allSFXPlayer.Add(_pitchedSFXLoopPlayer[pitchKey]);

                // TEMP
                InitialVolumeSetting();
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

            _pitchedSFXLoopPlayer[pitchKey].Stop();
            _pitchedSFXLoopPlayer[pitchKey].clip = clip;
            _pitchedSFXLoopPlayer[pitchKey].volume = volumeFactor;
            _pitchedSFXLoopPlayer[pitchKey].Play();
        }
        else
        {
            if (_pitchedSFXPlayer.ContainsKey(pitchKey) == false)
            {
                // Debug.Log($"_sfxPlayer => AudioSource AddComponent (pitchFactor: {pitchFactor})");

                _pitchedSFXPlayer[pitchKey] = _sfxPlayer.AddComponent<AudioSource>();
                _pitchedSFXPlayer[pitchKey].pitch = pitchFactor;

                if (allSFXPlayer.Contains(_pitchedSFXPlayer[pitchKey]) == false)
                    allSFXPlayer.Add(_pitchedSFXPlayer[pitchKey]);

                // TEMP
                InitialVolumeSetting();
            }

            _pitchedSFXPlayer[pitchKey].PlayOneShot(clip, volumeFactor);
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
        foreach (var audioSource in _pitchedSFXLoopPlayer.Values)
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

    public void PauseAllSound()
    {
        for (int i = 0; i < allSFXPlayer.Count; i++)
        {
            if (allSFXPlayer[i] == null)
            {
                allSFXPlayer.RemoveAt(i);
                continue;
            }

            allSFXPlayer[i].Pause();
        }

        _bgmPlayer.Pause();
    }
    public void UnPauseAllSound()
    {
        for (int i = 0; i < allSFXPlayer.Count; i++)
        {
            if (allSFXPlayer[i] == null)
            {
                allSFXPlayer.RemoveAt(i);
                continue;
            }

            allSFXPlayer[i].UnPause();
        }

        _bgmPlayer.UnPause();
    }

    public void OnSceneContextBuilt()
    {
        Debug.Log("OnSceneContextBuilt in SoundManager");

        allSFXPlayer = FindObjectsByType<AudioSource>(FindObjectsSortMode.None).ToList();
        allSFXPlayer.Remove(_bgmPlayer);
    }
}
