using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using UnityEngine;

public class DoorOpenAnimation : MonoBehaviour
{
    [SerializeField] private float _doorOpenDelay;
    [SerializeField] private float _stopShakeTiming = 2f;
    [SerializeField] private ConstantShakePreset _doorOpenPreset;

    [SerializeField] private SoundList _soundList;
    [SerializeField] private float _openSoundInterval;
    [SerializeField] private int _openSoundRepeat;
    [SerializeField] private float _preheatSoundInterval;
    [SerializeField] private int _preheadSoundRepeat;

    [SerializeField] ParticleHelper _dustParticle;

    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();   
    }
    public IEnumerator OpenCoroutine()
    {
        SceneEffectManager.Current.Camera.StartConstantShake(_doorOpenPreset);
        _dustParticle.Play();
        StartCoroutine(PlaySoundCoroutine("SE_LightDoor_Open_Low", _preheatSoundInterval, _preheadSoundRepeat));
        yield return new WaitForSeconds(_doorOpenDelay);
        _animator.SetTrigger("Open");
        StartCoroutine(PlaySoundCoroutine("SE_LightDoor_Open", _openSoundInterval, _openSoundRepeat));
        yield return new WaitForSeconds(_stopShakeTiming);
        _dustParticle.Stop();
        SceneEffectManager.Current.Camera.StopConstantShake();
    }
    private IEnumerator PlaySoundCoroutine(string key, float interval, int count)
    {
        for (int i = 0; i < count; i++)
        {
            _soundList.PlaySFX(key);
            yield return new WaitForSeconds(interval);
        }
    }
}
