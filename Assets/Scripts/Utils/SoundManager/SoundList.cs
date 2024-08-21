using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SoundClipData(Key, Clip, Volume, Pitch ..) �� ��Ƶΰ� �����Ѵ�
/// </summary>
public class SoundList : MonoBehaviour
{
    [SerializeField] private List<SoundClipData> _soundDatas;
    public List<SoundClipData> Datas => _soundDatas;

    // ���� Ŭ�� ������ '����Ʈ'�� O(1)�� �����ϱ� ���� ��ųʸ�
    private Dictionary<string, int> _soundIndexMap = new();

    private void Awake()
    {
        for (int index = 0; index < _soundDatas.Count; index++)
        {
            _soundIndexMap[_soundDatas[index].Key] = index;
        }
    }

    public void PlaySFX(string key, float pitchMultiplier = 1f, float volumeMultiplier = 1f)
    {
        // �ش� SoundList�� ����ص� Ű�� �����Ѵٸ�
        if (_soundIndexMap.ContainsKey(key))
        {
            var index = _soundIndexMap[key];
            var sound = _soundDatas[index];

            if (sound.Clip == null)
            {
                Debug.LogWarning("No clip for: " + key);
                return;
            }

            SoundManager.Instance.PlaySFX_AudioClip(sound.Clip, sound.Pitch * pitchMultiplier, sound.Volume * volumeMultiplier);
        }
        else
        {
            Debug.Log($"SoundList���� PlaySFX�� �õ������� �ش� 'Key'�� �������� �ʾ� SoundManager�� PlayCommonSFX�� �ѱ�ϴ�");

            // Ű�� �������� �ʴ´ٸ� SoundManager�� CommonSFX���� ����� �õ��Ѵ�
            SoundManager.Instance.PlayCommonSFX(key, pitchMultiplier, volumeMultiplier);
        }
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
