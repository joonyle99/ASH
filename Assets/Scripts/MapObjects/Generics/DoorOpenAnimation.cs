using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenAnimation : MonoBehaviour
{
    [SerializeField] float _doorOpenDelay;
    [SerializeField] float _stopShakeTiming = 2f;
    [SerializeField] ConstantShakePreset _doorOpenPreset;

    [SerializeField] SoundList _soundList;
    [SerializeField] float _openSoundInterval;
    [SerializeField] int _openSoundRepeat;
    [SerializeField] float _preheatSoundInterval;
    [SerializeField] int _preheadSoundRepeat;

    Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();   
    }
    public IEnumerator OpenCoroutine()
    {
        SceneEffectManager.Current.Camera.StartConstantShake(_doorOpenPreset);
        StartCoroutine(PlaySoundCoroutine("SE_LightDoor_Open_Low", _preheatSoundInterval, _preheadSoundRepeat));
        yield return new WaitForSeconds(_doorOpenDelay);
        _animator.SetTrigger("Open");
        StartCoroutine(PlaySoundCoroutine("SE_LightDoor_Open", _openSoundInterval, _openSoundRepeat));
        yield return new WaitForSeconds(_stopShakeTiming);
        SceneEffectManager.Current.Camera.StopConstantShake();
    }
    IEnumerator PlaySoundCoroutine(string key, float interval, int count)
    {
        for (int i = 0; i < count; i++)
        {
            _soundList.PlaySFX(key);
            yield return new WaitForSeconds(interval);
        }
    }
}
