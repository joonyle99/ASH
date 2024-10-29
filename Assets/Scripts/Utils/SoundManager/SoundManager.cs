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

    [SerializeField] private AudioMixer _audioMixer;

    [Space]

    public List<AudioSource> allSFXPlayer;

    private const int PitchPrecision = 1000;

    private SoundList[] _soundListMap; // ui, bgm, gimmick ...

    private Dictionary<string, int> _soundListIndexMap = new();
    private Dictionary<int, AudioSource> _pitchedSFXPlayer = new();

    protected override void Awake()
    {
        base.Awake();

        _soundListMap = _soundListParent.GetComponentsInChildren<SoundList>();

        // each 'sound list'
        for (int i = 0; i < _soundListMap.Length; i++)
        {
            // each 'sound data' in sound list
            for (int j = 0; j < _soundListMap[i].Datas.Count; j++)
            {
                var soundDataKey = _soundListMap[i].Datas[j].Key;
                _soundListIndexMap[soundDataKey] = i;
            }
        }

        _pitchedSFXPlayer[PitchPrecision] = _sfxPlayer;
    }

    private void Update()
    {
#if UNITY_EDITOR
        /*
         * TEST CODE
         * 
        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (var sfxPlayer in _pitchedSFXPlayer)
            {
                Debug.Log($"_pitchedSFXPlayer => <color=orange>Key</color>: {sfxPlayer.Key}, <color=yellow>Value</color>: {sfxPlayer.Value}", sfxPlayer.Value.gameObject);
            }
        }
        */
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
    public void PlaySFX_SoundClipData(SoundClipData soundData, float pitchMultiplier = 1f, float volumeMultiplier = 1f)
    {
        PlaySFX_AudioClip(soundData.Clip, soundData.Pitch * pitchMultiplier, soundData.Volume * volumeMultiplier);
    }
    public void PlaySFX_AudioClip(AudioClip clip, float pitchFactor, float volumeFactor)
    {
        // 음수의 PitchFactor는 허용하지 않음
        if (pitchFactor < 0f)
            pitchFactor = 0.001f;

        int pitchKey = Mathf.RoundToInt(pitchFactor * PitchPrecision);

        if (_pitchedSFXPlayer.ContainsKey(pitchKey) == false)
        {
            _pitchedSFXPlayer[pitchKey] = _sfxPlayer.AddComponent<AudioSource>();
            _pitchedSFXPlayer[pitchKey].pitch = pitchFactor;

            if (allSFXPlayer.Contains(_pitchedSFXPlayer[pitchKey]) == false)
                allSFXPlayer.Add(_pitchedSFXPlayer[pitchKey]);

            InitialVolumeSetting();
        }

        _pitchedSFXPlayer[pitchKey].PlayOneShot(clip, volumeFactor);
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
        if (_soundListIndexMap.TryGetValue(key, out var soundListIndex))
        {
            // SoundManager의 SoundList에 접근하여 해당 키에 맞는 SFX를 재생
            _soundListMap[soundListIndex].PlaySFX(key, pitchMultiplier, volumeMultiplier);
            return;
        }

        Debug.LogWarning("No SFX matching: " + key);
    }
    public void PlayCommonBGM(string key, float volumeMultiplier = 1f)
    {
        if (_soundListIndexMap.TryGetValue(key, out var soundListIndex))
        {
            _soundListMap[soundListIndex].PlayBGM(key, volumeMultiplier);
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
        else if (GameSceneManager.IsEndingScene(sceneName))
        {
            PlayCommonBGM("EndingTheme");
        }
        else
        {
            // 기본 BGM 재생
            PlayCommonBGM("BasicBGM");
        }
    }

    // stop bgm sound
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
        // Debug.Log("OnSceneContextBuilt in SoundManager");

        allSFXPlayer = FindObjectsByType<AudioSource>(FindObjectsSortMode.None).ToList();
        allSFXPlayer.Remove(_bgmPlayer);
    }
}
