using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class LightController : MonoBehaviour
{
    [SerializeField] private GameObject _light;
    [SerializeField] private GameObject _cane;

    [Space]

    [SerializeField] private bool _isLightableState;
    [SerializeField] private bool _isLightWorking;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _maxAngle;
    [SerializeField] private float _curAngle;
    [SerializeField] private float _lightAngleValue;
    [SerializeField] private bool _isRotatingUp;

    public float PlayerDir { get => this.transform.localScale.x; }

    PlayerBehaviour _player;

    private void Awake()
    {

        _player = GetComponent<PlayerBehaviour>();
    }
    void Update()
    {
        _isLightableState = _player.StateIs<IdleState>() || _player.StateIs<RunState>();

        if (_isLightWorking)
        {
            if (!_isLightableState)
            {
                TurnOffLight();
                _isLightWorking = _light.activeSelf;
            }
        }

        InputState inputState = InputManager.Instance.GetState();

        // Light Source ON / OFF
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (_isLightableState)
                _isLightWorking = !_isLightWorking;
        }

        // -35 ~ 35를 0 ~ 1로 정규화
        _lightAngleValue = (_curAngle + _maxAngle) / (2 * _maxAngle);

        // Animator Parameter
        _player.Animator.SetBool("IsLightWorking", _isLightWorking);
        _player.Animator.SetFloat("LightAngleValue", _lightAngleValue);

        // Light Source Up / Down Rotations
        if (_isLightWorking)
        {
            _light.transform.Rotate(Vector3.forward, (PlayerDir > 0f ? _rotateSpeed : -_rotateSpeed) * inputState.Vertical * Time.deltaTime);

            // 상한선 하한선 정하기

            // 상한선
            if (_light.transform.localEulerAngles.z > _maxAngle && _light.transform.localEulerAngles.z < 90f)
            {
                _light.transform.localEulerAngles = new Vector3(_light.transform.localEulerAngles.x,
                    _light.transform.localEulerAngles.y, _maxAngle);
                _curAngle = _maxAngle;
            }
            // 0 ~ maxAngle
            else if (_light.transform.localEulerAngles.z > 0f && _light.transform.localEulerAngles.z < _maxAngle)
            {
                _curAngle = _light.transform.localEulerAngles.z;
                _isRotatingUp = true;
            }
            // 하한선
            else if (_light.transform.localEulerAngles.z > 270f && _light.transform.localEulerAngles.z < 360f - _maxAngle)
            {
                _light.transform.localEulerAngles = new Vector3(_light.transform.localEulerAngles.x,
                    _light.transform.localEulerAngles.y, 360f - _maxAngle);
                _curAngle = -_maxAngle;
            }
            // -maxAngle ~ 0
            else if (_light.transform.localEulerAngles.z > 360f - _maxAngle && _light.transform.localEulerAngles.z < 360f)
            {
                _curAngle = -(360f - _light.transform.localEulerAngles.z);
                _isRotatingUp = false;
            }
        }
    }

    public void TurnOnLight()
    {
        Vector3 originLocalPosition = _light.transform.localPosition;
        Quaternion originLocalRotation = _light.transform.localRotation;
        Vector3 originLocalScale = _light.transform.localScale;

        // 빛을 켠다
        _light.SetActive(true);
        _player.SoundList.PlaySFX("SE_LightSkill");

        // 지팡이의 자식으로 붙힌다.
        _light.transform.SetParent(_cane.transform.GetChild(0).transform);

        _light.transform.localPosition = originLocalPosition;
        _light.transform.localRotation = originLocalRotation;
        _light.transform.localScale = originLocalScale;
    }

    public void TurnOffLight()
    {
        Vector3 originLocalPosition = _light.transform.localPosition;
        Quaternion originLocalRotation = _light.transform.localRotation;
        Vector3 originLocalScale = _light.transform.localScale;

        // 플레이어 자식으로 붙힌다.
        _light.transform.SetParent(transform);

        _light.transform.localPosition = originLocalPosition;
        _light.transform.localRotation = originLocalRotation;
        _light.transform.localScale = originLocalScale;

        // 빛을 끈다
        _light.SetActive(false);
    }
}
