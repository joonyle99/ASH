using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using UnityEngine;

public class DoorOpenAnimation : MonoBehaviour
{
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
        SceneEffectManager.Instance.Camera.StartConstantShake(_doorOpenPreset);
        _dustParticle.Play();
        _soundList.PlaySFX("SE_LightDoor_Open_Low");

        yield return new WaitForSeconds(_doorOpenDelay);

        // 2
        _animator.SetTrigger("Open");
        _soundList.PlaySFX("SE_LightDoor_Open");

        yield return new WaitForSeconds(_stopShakeTiming);

        // 3
        _dustParticle.Stop();
        SceneEffectManager.Instance.Camera.StopConstantShake();
    }
    public IEnumerator CloseCoroutine()
    {
        // 1
        SceneEffectManager.Instance.Camera.StartConstantShake(_doorOpenPreset);
        _dustParticle.Play();
        _soundList.PlaySFX("SE_LightDoor_Open_Low");

        yield return new WaitForSeconds(_doorOpenDelay);

        // 2
        _animator.SetTrigger("Close");
        _soundList.PlaySFX("SE_LightDoor_Open");

        yield return new WaitForSeconds(_stopShakeTiming);

        // 3
        _dustParticle.Stop();
        SceneEffectManager.Instance.Camera.StopConstantShake();
    }
}
