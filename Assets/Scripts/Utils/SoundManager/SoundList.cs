using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundList : MonoBehaviour
{
    [SerializeField] List<SoundClipData> _soundDatas;

    public List<SoundClipData> Datas => _soundDatas;

    Dictionary<string, int> _soundMap = new Dictionary<string, int>();

    private void Awake()
    {
        for(int i=0; i<_soundDatas.Count; i++)
        {
            _soundMap[_soundDatas[i].Key] = i;
        }
    }

    public void PlaySFX(string key, float volumeMultiplier = 1f)
    {
        if(_soundMap.ContainsKey(key))
        {
            var sound = _soundDatas[_soundMap[key]];
            if (sound.Clip == null)
                Debug.LogWarning("No clip for: " + key);
            else
                SoundManager.Instance.PlaySFXPitched(sound.Clip, sound.Pitch, sound.Volume * volumeMultiplier);
        }
        else
        {
            SoundManager.Instance.PlayCommonSFXPitched(key, 1, volumeMultiplier);
        }
    }
    public void PlaySFXPitched(string key, float pitchMultiplier, float volumeMultiplier = 1f)
    {
        if (_soundMap.ContainsKey(key))
        {
            var sound = _soundDatas[_soundMap[key]];
            if (sound.Clip == null)
                Debug.LogWarning("No clip for: " + key);
            else
                SoundManager.Instance.PlaySFXPitched(sound.Clip, sound.Pitch * pitchMultiplier, sound.Volume * volumeMultiplier);
        }
        else
        {
            SoundManager.Instance.PlayCommonSFXPitched(key, pitchMultiplier, volumeMultiplier);
        }
    }
    public void PlayBGM(string key, float volumeMultiplier = 1f)
    {
        if (_soundMap.ContainsKey(key))
        {
            var sound = _soundDatas[_soundMap[key]];
            if (sound.Clip == null)
                Debug.LogWarning("No clip for: " + key);
            else
                SoundManager.Instance.PlayBGM(sound.Clip, sound.Volume * volumeMultiplier);
        }
        else
        {
            SoundManager.Instance.PlayCommonBGM(key, volumeMultiplier);
        }
    }

    public bool Exists(string key)
    {
        return _soundDatas.Exists(x=>x.Key == key);
    }
}
