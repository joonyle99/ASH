using System.Collections;
using UnityEngine;

public class HiddenPath : MonoBehaviour, ILightCaptureListener
{
    [SerializeField] HiddenPathMask _mask;
    [SerializeField] HiddenPathMask.Direction _swipeDirection;
    [SerializeField] float _swipeDuration;
    [SerializeField] GameObject _destroyingCollidersParent;
    [SerializeField] InputSetterScriptableObject _openInputSetter;
    [SerializeField] float _requiredLightTime = 0.7f;
    [SerializeField] float _openDelay = 0.7f;
    [SerializeField] SoundList _soundList;
    [SerializeField] GameObject _lightEffect;
    [SerializeField] GameObject _lightCapturer;

    PreserveState _statePreserver;

    float _eTime = 0f;
    void Awake()
    {
        _mask.InitMask(_swipeDirection);
        _statePreserver = GetComponent<PreserveState>();
        if (_statePreserver)
        {
            if (!_statePreserver.LoadState<bool>("_isOpenSaved", false))
            {
                //단순 씬전환이 일어난 경우
                if(_statePreserver.LoadState("_isOpen", false))
                {
                    Destroy(_lightCapturer);
                    Destroy(_destroyingCollidersParent);
                    _lightEffect.SetActive(false);
                    _mask.InstantReveal();
                }
            }//저장된 데이터 불러와지는 경우
            else
            {
                Destroy(_lightCapturer);
                Destroy(_destroyingCollidersParent);
                _lightEffect.SetActive(false);
                _mask.InstantReveal();
            }
        }

        SaveAndLoader.OnSaveStarted += SaveOpenState;
    }
    
    void OnDestroy()
    {
        if(_statePreserver && !SaveAndLoader.IsChangeSceneByLoading)
        {
            _statePreserver.SaveState("_isOpen", _lightCapturer == null);
        }
    }

    IEnumerator OpenPathCoroutine()
    {
        InputManager.Instance.ChangeInputSetter(_openInputSetter);
        _soundList.PlaySFX("SE_HiddenPath_Contact");
        yield return new WaitForSeconds(_openDelay);
        _soundList.PlaySFX("SE_HiddenPath_Open");
        _lightEffect.SetActive(false);
        _mask.OnLightCaptured(_swipeDuration);
        InputManager.Instance.ChangeToDefaultSetter();
    }
    public void OnLightStay(LightCapturer capturer, LightSource lightSource)
    {
        _eTime += Time.deltaTime;
        if (_eTime >= _requiredLightTime)
        {
            StartCoroutine(OpenPathCoroutine());
            Destroy(capturer.gameObject);
            Destroy(_destroyingCollidersParent);
        }
    }
    public void OnLightExit(LightCapturer capturer, LightSource lightSource)
    {
        _eTime = 0f;
    }

    private void SaveOpenState()
    {
        if (_statePreserver)
        {
            _statePreserver.SaveState("_isOpenSaved", _lightCapturer == null);
        }
    }

}
