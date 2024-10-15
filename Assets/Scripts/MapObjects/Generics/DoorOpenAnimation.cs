using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DoorOpenAnimation : MonoBehaviour
{
    [SerializeField] private string _openLowSoundKey = "SE_LightDoor_Open_Low";
    [SerializeField] private string _openSoundKey = "SE_LightDoor_Open";

    [SerializeField] private float _doorOpenDelay;
    [SerializeField] private float _stopShakeTiming = 2f;
    [SerializeField] private ConstantShakePreset _doorOpenPreset;

    [SerializeField] private SoundList _soundList;

    [SerializeField] ParticleHelper _dustParticle;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public IEnumerator OpenCoroutine()
    {
        // 1
        if(_doorOpenPreset)
            SceneEffectManager.Instance.Camera.StartConstantShake(_doorOpenPreset);

        _dustParticle?.Play();

        if (_openLowSoundKey.Length != 0)
            _soundList.PlaySFX(_openLowSoundKey);

        yield return new WaitForSeconds(_doorOpenDelay);

        // 2
        _animator.SetTrigger("Open");

        if (_openSoundKey.Length != 0)
            _soundList.PlaySFX(_openSoundKey);

        yield return new WaitForSeconds(_stopShakeTiming);

        // 3
        _dustParticle?.Stop();

        if (_doorOpenPreset)
            SceneEffectManager.Instance.Camera.StopConstantShake();
    }

    public IEnumerator CloseCoroutine()
    {
        // 1
        if (_doorOpenPreset)
            SceneEffectManager.Instance.Camera.StartConstantShake(_doorOpenPreset);

        _dustParticle?.Play();

        if (_openLowSoundKey.Length != 0)
            _soundList.PlaySFX(_openLowSoundKey);

        yield return new WaitForSeconds(_doorOpenDelay);

        // 2
        _animator.SetTrigger("Close");

        if (_openSoundKey.Length != 0)
            _soundList.PlaySFX(_openSoundKey);

        yield return new WaitForSeconds(_stopShakeTiming);

        // 3
        _dustParticle?.Stop();

        if (_doorOpenPreset)
            SceneEffectManager.Instance.Camera.StopConstantShake();
    }
}
