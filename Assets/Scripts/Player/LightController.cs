using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField] private GameObject _flashLight;
    [SerializeField] private GameObject _cane;

    [Space]

    [SerializeField] private bool _isChangeableToLightState;
    [SerializeField] private bool _isLightWorking;
    [SerializeField] private float _rotateSpeed = 30f;
    [SerializeField] private float _maxAngle = 35f;
    [SerializeField] private float _curAngle;
    [SerializeField] private bool _isRotatingUp;

    public float PlayerDir { get => this.transform.localScale.x; }

    void Update()
    {
        PlayerBehaviour playerBehaviour = this.GetComponent<PlayerBehaviour>();
        _isChangeableToLightState = playerBehaviour.StateIs<IdleState>() || playerBehaviour.StateIs<RunState>();

        if (_isLightWorking)
        {
            if (!_isChangeableToLightState)
            {
                TurnOffLight();
                _isLightWorking = _flashLight.activeSelf;
            }
        }

        InputState inputState = InputManager.Instance.GetState();

        // Light Source ON / OFF
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (_isChangeableToLightState)
                _isLightWorking = !_isLightWorking;
        }

        // Animator Parameter
        playerBehaviour.Animator.SetBool("IsLightWorking", _isLightWorking);
        playerBehaviour.Animator.SetFloat("LightAngle", (_curAngle + _maxAngle) / (2 * _maxAngle));

        // Light Source Up / Down Rotations
        if (_isLightWorking)
        {
            _flashLight.transform.Rotate(Vector3.forward, (PlayerDir > 0f ? _rotateSpeed : -_rotateSpeed) * inputState.Vertical * Time.deltaTime);

            // 상한선 하한선 정하기

            // 상한선
            if (_flashLight.transform.localEulerAngles.z > _maxAngle && _flashLight.transform.localEulerAngles.z < 90f)
            {
                _flashLight.transform.localEulerAngles = new Vector3(_flashLight.transform.localEulerAngles.x,
                    _flashLight.transform.localEulerAngles.y, _maxAngle);
                _curAngle = _maxAngle;
            }
            // 0 ~ maxAngle
            else if (_flashLight.transform.localEulerAngles.z > 0f && _flashLight.transform.localEulerAngles.z < _maxAngle)
            {
                _curAngle = _flashLight.transform.localEulerAngles.z;
                _isRotatingUp = true;
            }
            // 하한선
            else if (_flashLight.transform.localEulerAngles.z > 270f && _flashLight.transform.localEulerAngles.z < 360f - _maxAngle)
            {
                _flashLight.transform.localEulerAngles = new Vector3(_flashLight.transform.localEulerAngles.x,
                    _flashLight.transform.localEulerAngles.y, 360f - _maxAngle);
                _curAngle = -_maxAngle;
            }
            // -maxAngle ~ 0
            else if (_flashLight.transform.localEulerAngles.z > 360f - _maxAngle && _flashLight.transform.localEulerAngles.z < 360f)
            {
                _curAngle = -(360f - _flashLight.transform.localEulerAngles.z);
                _isRotatingUp = false;
            }
        }
    }

    public void TurnOnLight()
    {
        Vector3 originLocalPosition = _flashLight.transform.localPosition;
        Quaternion originLocalRotation = _flashLight.transform.localRotation;
        Vector3 originLocalScale = _flashLight.transform.localScale;

        // 빛을 켠다
        _flashLight.SetActive(true);

        // 지팡이의 자식으로 붙힌다.
        _flashLight.transform.SetParent(_cane.transform.GetChild(0).transform);

        _flashLight.transform.localPosition = originLocalPosition;
        _flashLight.transform.localRotation = originLocalRotation;
        _flashLight.transform.localScale = originLocalScale;
    }

    public void TurnOffLight()
    {
        Vector3 originLocalPosition = _flashLight.transform.localPosition;
        Quaternion originLocalRotation = _flashLight.transform.localRotation;
        Vector3 originLocalScale = _flashLight.transform.localScale;

        // 플레이어 자식으로 붙힌다.
        _flashLight.transform.SetParent(transform);

        _flashLight.transform.localPosition = originLocalPosition;
        _flashLight.transform.localRotation = originLocalRotation;
        _flashLight.transform.localScale = originLocalScale;

        // 빛을 끈다
        _flashLight.SetActive(false);
    }
}
