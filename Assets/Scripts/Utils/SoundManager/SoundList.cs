using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SoundClipData(Key, Clip, Volume, Pitch ..) 를 담아두고 관리한다
/// </summary>
[System.Serializable]
public class SoundList : MonoBehaviour
{
    [SerializeField] private List<SoundClipData> _soundDatas;
    public List<SoundClipData> Datas => _soundDatas;

    // 사운드 클립 데이터 '리스트'에 O(1)로 접근하기 위한 딕셔너리
    private Dictionary<string, int> _soundIndexMap = new();

    [SerializeField] private Coroutine _recentLoopPlayedSFX;

    private void Awake()
    {
        for (int index = 0; index < _soundDatas.Count; index++)
        {
            _soundIndexMap[_soundDatas[index].Key] = index;
        }
    }

    public void PlaySFX(string key, float pitchMultiplier = 1f, float volumeMultiplier = 1f)
    {
        // 해당 SoundList에 등록해둔 키가 존재한다면
        if (_soundIndexMap.ContainsKey(key))
        {
            var index = _soundIndexMap[key];
            var sound = _soundDatas[index];

            if (sound.Clip == null)
            {
                Debug.LogWarning("No clip for: " + key);
                return;
            }

            if(sound.IsLoop)
            {
                _recentLoopPlayedSFX = StartCoroutine(PlaySFXRequestCoroutine(sound));
            }
            else
            {
                SoundManager.Instance.PlaySFX_AudioClip(sound.Clip, sound.Pitch * pitchMultiplier, sound.Volume * volumeMultiplier);
                //Debug.Log("Play sound at PlaySFX_AudioClip()");
            }
        }
        else
        {
            // 키가 존재하지 않는다면 SoundManager의 CommonSFX에서 재생을 시도한다
            SoundManager.Instance.PlayCommonSFX(key, pitchMultiplier, volumeMultiplier);
            Debug.Log("Play sound at PlayCommonSFX()");
        }
    }

    private IEnumerator PlaySFXRequestCoroutine(SoundClipData sound, float pitchMultiplier = 1f, float volumeMultiplier = 1f)
    {
        while(true)
        {
            SoundManager.Instance.PlaySFX_AudioClip(sound.Clip, sound.Pitch * pitchMultiplier, sound.Volume * volumeMultiplier);

            yield return new WaitForSeconds(sound.Clip.length);
        }
    }

    public void StopRecentLoopPlayedSFX()
    {
        if (_recentLoopPlayedSFX == null)
            return;

        StopCoroutine(_recentLoopPlayedSFX);
    }

    public void PlayBGM(string key, float volumeMultiplier = 1f)
    {
        if (_soundIndexMap.ContainsKey(key))
        {
            var index = _soundIndexMap[key];
            var sound = _soundDatas[index];
            if (sound.Clip == null)
            {
                Debug.LogWarning("No clip for: " + key);
            }
            else
            {
                SoundManager.Instance.PlayBGM(sound.Clip, sound.Volume * volumeMultiplier);
            }
        }
        else
        {
            SoundManager.Instance.PlayCommonBGM(key, volumeMultiplier);
        }
    }

    public bool Exists(string key)
    {
        return _soundDatas.Exists(x => x.Key == key);
    }
}
