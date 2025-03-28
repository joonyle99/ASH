using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : HappyTools.SingletonBehaviourFixed<SoundManager>, ISceneContextBuildListener
{
    [SerializeField] private GameObject _soundListParent;

    private List<AudioSource> _playingBgmPlayers = new List<AudioSource>();

    //해당 게임 오브젝트에 bgm으로 사용할 audio source 추가
    [SerializeField] private GameObject _bgmPlayer;

    [SerializeField] private AudioSource _sfxPlayer;

    [SerializeField] private AudioMixer _audioMixer;

    [Space]

    public List<AudioSource> allSFXPlayer;

    private const int PitchPrecision = 1000;

    private SoundList[] _soundLists; // ui, bgm, gimmick, skill ...

    private Dictionary<string, int> _soundListIndexMap = new();
    private Dictionary<int, AudioSource> _pitchedSFXPlayer = new();

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
    }

#if UNITY_EDITOR
    private void Update()
    {
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
    }
#endif

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

            for (int i = 0; i < _playingBgmPlayers.Count; i++)
            {
                float originDataVolume = GetSoundClipData(_playingBgmPlayers[i].clip).Volume;
                _playingBgmPlayers[i].mute = true;
                _playingBgmPlayers[i].volume = volume * originDataVolume;
            }
        }
        else
        {
            _audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);

            for (int i = 0; i < _playingBgmPlayers.Count; i++)
            {
                float originDataVolume = GetSoundClipData(_playingBgmPlayers[i].clip).Volume;
                _playingBgmPlayers[i].mute = false;
                _playingBgmPlayers[i].volume = volume * originDataVolume;
            }
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
            {
                allSFXPlayer.Add(_pitchedSFXPlayer[pitchKey]);
            }

            InitialVolumeSetting();
        }

        _pitchedSFXPlayer[pitchKey].PlayOneShot(clip, volumeFactor);
    }
    public void PlayBGM(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (CheckPlayingClip(clip))
        {
            Debug.Log($"Already Playing this Audio Clip" +
                             $"\n" +
                             $"<color=yellow>New Audio Source</color>: {clip.name}");
            return;
        }

        AudioSource currentBgmPlayer = GetRestBgmPlayer();
        currentBgmPlayer.Stop();
        currentBgmPlayer.clip = clip;
        currentBgmPlayer.volume = volumeMultiplier;
        currentBgmPlayer.Play();
        _playingBgmPlayers.Add(currentBgmPlayer);
    }

    // play common sound (searching in sound list which sound manager has)
    public void PlayCommonSFX(string key, float pitchMultiplier = 1f, float volumeMultiplier = 1f)
    {
        if (_soundListIndexMap.TryGetValue(key, out var soundListIndex))
        {
            // SoundManager의 SoundList에 접근하여 해당 키에 맞는 SFX를 재생
            //Debug.Log("PlayCommonSFX: " + key);
            _soundLists[soundListIndex].PlaySFX(key, pitchMultiplier, volumeMultiplier);
            return;
        }

        Debug.LogWarning("No SFX matching: " + key);
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

    /// <summary>
    /// 일반적으로 인자로 들어온 bgm을 사용중이지 않는 오디오소스에 페이드 인 하며 재생,
    /// targetSource를 지정해 주게 되면 해당 오디오 소스를 사용하여 페이드 인 하며 재생
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="targetSource">null이 아닌 경우 특정 오디오 소스를 사용, audioSource를 
    /// 지정해 주면 _playingBgmPlayer에 대한 전처리 및 후처리 해야함</param>
    /// <returns></returns>
    public bool PlayCommonBGMFade(string key, float duration, AudioSource newBgmPlayer = null, float startTime = 0)
    {
        if (!_soundListIndexMap.TryGetValue(key, out var soundListIndex))
        {
            Debug.LogWarning("No BGM matching: " + key);
            return false;
        }

        SoundClipData newClip = _soundLists[soundListIndex].GetSoundClipData(key);
        if (CheckPlayingClip(newClip.Clip))
        {
            Debug.Log($"Already playing clip : {newClip.Clip}");

            return false;
        }

        if (newBgmPlayer == null)
        {
            newBgmPlayer = GetRestBgmPlayer();
        }

        StartCoroutine(BGMFadeInCoroutine(key, newClip, duration, newBgmPlayer, startTime));

        return true;
    }

    private IEnumerator BGMFadeInCoroutine(string key, SoundClipData soundClipData, float duration, AudioSource newBgmPlayer, float startTime)
    {
        float eTime = 0f;
        newBgmPlayer.clip = soundClipData.Clip;
        newBgmPlayer.time = startTime;
        newBgmPlayer.volume = 0;
        newBgmPlayer.Play();
        //Debug.Log($"key : {key} startTime {startTime} player's time {newBgmPlayer.time}");

        while (eTime < duration)
        {
            eTime += Time.deltaTime;

            yield return null;

            //설정에서 볼륨이 바뀐 경우 해당 볼륨까지 도달했는지 체크
            if (!newBgmPlayer.isPlaying || newBgmPlayer.volume >= GetAudioMixerVolume("BGM") * GetSoundClipData(key).Volume)
                break;

            float t = eTime / duration;
            newBgmPlayer.volume = Mathf.Lerp(0, GetAudioMixerVolume("BGM") * GetSoundClipData(key).Volume, t);
        }

        newBgmPlayer.volume = GetAudioMixerVolume("BGM") * GetSoundClipData(key).Volume;
        _playingBgmPlayers.Add(newBgmPlayer);
    }

    public void BGMFadeInOut(string fadeinKey, float fadeinDuration,
        string exceptionFadeoutKey, float fadeoutDuration,
        float volumeMultiplier = 1f)
    {
        bool isExceptionFadeout = exceptionFadeoutKey.Length != 0;
        int exceptionSoundListIndex = -1;

        if (!_soundListIndexMap.TryGetValue(fadeinKey, out var fadeinSoundListIndex))
        {
            Debug.LogWarning("No BGM matching with fadein: " + fadeinKey);
            return;
        }

        if (isExceptionFadeout && !_soundListIndexMap.TryGetValue(exceptionFadeoutKey, out exceptionSoundListIndex))
        {
            Debug.LogWarning("No BGM matching with fadeout: " + exceptionFadeoutKey);
            return;
        }

        if (PlayCommonBGMFade(fadeinKey, fadeinDuration, GetRestBgmPlayer()))
        {
            SoundClipData exceptionFadeoutClipData =
                isExceptionFadeout ? _soundLists[exceptionSoundListIndex].GetSoundClipData(exceptionFadeoutKey) : null;
            SoundClipData newClip = _soundLists[fadeinSoundListIndex].GetSoundClipData(fadeinKey);

            for (int i = 0; i < _playingBgmPlayers.Count; i++)
            {
                if (!(isExceptionFadeout && exceptionFadeoutClipData.Clip == _playingBgmPlayers[i].clip) &&
                    (newClip.Clip != _playingBgmPlayers[i].clip))
                {
                    StopBGMFade(fadeoutDuration, _playingBgmPlayers[i]);
                }
            }
        }
    }
    public void PlayCommonBGMForScene(string sceneName)
    {
        if (GameSceneManager.IsOpeningScene(sceneName))
        {
            BGMFadeInOut("MainTheme", 5, "Dungeon_Wave", 5);
            //PlayCommonBGM("MainTheme");
        }
        else if (GameSceneManager.IsExploration1(sceneName))
        {
            BGMFadeInOut("Exploration1", 5, "Dungeon_Wave", 5, 1);
            //PlayCommonBGM("Exploration1");
        }
        else if (GameSceneManager.IsExploration2(sceneName))
        {
            BGMFadeInOut("Exploration2", 5, "Dungeon_Wave", 5);
            //PlayCommonBGM("Exploration2");
        }
        else if (GameSceneManager.IsBossDungeon1(sceneName)
            || GameSceneManager.IsBossDungeon2(sceneName)
            || GameSceneManager.IsBossScene(sceneName))
        {
            BGMFadeInOut("BossDungeon1", 5, "Dungeon_Wave", 5);
            //PlayCommonBGM("BossDungeon1");
        }
        else if (GameSceneManager.IsEndingScene(sceneName))
        {
            BGMFadeInOut("EndingTheme", 5, "Dungeon_Wave", 5);
            //PlayCommonBGM("EndingTheme");
        }
        else
        {
            BGMFadeInOut("BasicBGM", 5, "Dungeon_Wave", 5);
            // 기본 BGM 재생
            //PlayCommonBGM("BasicBGM");
        }
    }

    // stop bgm sound
    public void StopBGM()
    {
        for (int i = 0; i < _playingBgmPlayers.Count; i++)
        {
            _playingBgmPlayers[i].Stop();
        }
    }
    /// <summary>
    /// 일반적으로 현재 플레이 중인 bgm을 페이드 아웃하여 멈춤,
    /// targetSource를 지정해 주게 되면 해당 오디오 소스를 페이드 아웃 하며 멈춤
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="targetSource">null이 아닌 경우 특정 오디오 소스를 사용, audioSource를 
    /// 지정해 주면 _playingBgmPlayer에 대한 전처리 및 후처리 해야함</param>
    /// <returns></returns>
    public void StopBGMFade(float duration, AudioSource targetSource = null)
    {
        if (targetSource == null)
        {
            Debug.Log($"Want to stop fadeout target player is null");
            return;
        }

        StartCoroutine(BGMFadeOutCoroutine(duration, targetSource));
    }

    private IEnumerator BGMFadeOutCoroutine(float duration, AudioSource targetSource)
    {
        float eTime = 0f;
        float originalChannelVolume = targetSource.volume;

        while (eTime < duration)
        {
            eTime += Time.deltaTime;

            yield return null;

            if (!targetSource.isPlaying)
                break;

            float t = eTime / duration;
            targetSource.volume = Mathf.Lerp(originalChannelVolume, 0, t);
        }

        if (targetSource.isPlaying)
        {
            targetSource.Stop();
        }

        _playingBgmPlayers.Remove(targetSource);
    }

    private AudioSource GetRestBgmPlayer()
    {
        AudioSource[] bgmSources = _bgmPlayer.GetComponents<AudioSource>();
        for (int i = 0; i < bgmSources.Length; i++)
        {
            if (!bgmSources[i].isPlaying)
            {
                return bgmSources[i];
            }
        }

        AudioSource newSource = _bgmPlayer.AddComponent<AudioSource>();
        newSource.loop = true;
        return newSource;
    }

    //특정 clip을 재생하고 있는 bgmPlayer를 호출한다
    public AudioSource GetBgmPlayer(string key)
    {
        SoundClipData clip = GetSoundClipData(key);
        if (clip == null)
            return null;


        for (int i = 0; i < _playingBgmPlayers.Count; i++)
        {
            //Debug.Log($"Checking {i} bgm player, played clip {_playingBgmPlayers[i].clip}");
            if (_playingBgmPlayers[i].clip == clip.Clip)
            {
                return _playingBgmPlayers[i];
            }
        }

        return null;
    }

    //특정 클립의 SoundClipData를 호출한다
    public SoundClipData GetSoundClipData(string key)
    {
        if (!_soundListIndexMap.TryGetValue(key, out int idx))
        {
            return null;
        }

        return _soundLists[idx].GetSoundClipData(key);
    }

    //특정 클립의 SoundClipData를 호출한다
    public SoundClipData GetSoundClipData(AudioClip clip)
    {
        for (int i = 0; i < _soundListIndexMap.Count; i++)
        {
            SoundClipData clipData = _soundLists[i].GetSoundClipData(clip);

            if(clipData != null)
            {
                return clipData;
            }
        }

        return null;
    }

    private bool CheckPlayingClip(AudioClip clip)
    {
        for (int i = 0; i < _playingBgmPlayers.Count; i++)
        {
            if (_playingBgmPlayers[i].clip == clip)
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// 설정에 의한 볼륨 값은 soundMixer에 값이 보간되어 들어감,
    /// 0-1사이의 값으로 역보간한 수치를 리턴
    /// </summary>
    /// <returns></returns>
    private float GetAudioMixerVolume(string key)
    {
        _audioMixer.GetFloat(key, out float value);
        value = Mathf.Pow(10, value / 20);

        return value;
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

        for (int i = 0; i < _playingBgmPlayers.Count; i++)
        {
            _playingBgmPlayers[i].Pause();
        }
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

        for (int i = 0; i < _playingBgmPlayers.Count; i++)
        {
            _playingBgmPlayers[i].UnPause();
        }
    }

    public void OnSceneContextBuilt()
    {
        // Debug.Log("OnSceneContextBuilt in SoundManager");

        allSFXPlayer = FindObjectsByType<AudioSource>(FindObjectsSortMode.None).ToList();
        //allSFXPlayer.Remove(_mainBgmPlayer);
    }
}
