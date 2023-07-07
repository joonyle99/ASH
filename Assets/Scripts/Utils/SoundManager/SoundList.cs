using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundList : MonoBehaviour
{
    [SerializeField] List<SoundClipData> _soundDatas;

    Dictionary<string, SoundClipData> _soundMap = new Dictionary<string, SoundClipData>();

    private void Awake()
    {
        foreach(SoundClipData data in _soundDatas)
        {
            _soundMap[data.Key] = data;
        }
    }
    public void PlaySFX(string key, float volumeMultiplier = 1f)
    {
        print("1");
        if(_soundMap.ContainsKey(key))
        {
            print("@");
            var sound = _soundMap[key];
            SoundManager.Instance.PlaySFXPitched(sound.Clip, sound.Pitch, sound.Volume * volumeMultiplier);
        }
        else
        {
            //Common 에서 재생
        }
    }
    public void PlaySFXPitched(string key, float pitchMultiplier, float volumeMultiplier = 1f)
    {
        if (_soundMap.ContainsKey(key))
        {
            var sound = _soundMap[key];
            SoundManager.Instance.PlaySFXPitched(sound.Clip, sound.Pitch * pitchMultiplier, sound.Volume * volumeMultiplier);
        }
        else
        {
            //Common 에서 재생
        }
    }
    public void PlayBGM(string key, float volumeMultiplier = 1f)
    {
        if (_soundMap.ContainsKey(key))
        {
            var sound = _soundMap[key];
            SoundManager.Instance.PlayBGM(sound.Clip, sound.Volume * volumeMultiplier);
        }
        else
        {
            //Common 에서 재생
        }
    }
}
