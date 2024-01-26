using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField] private GameObject _light;

    [Space]

    [SerializeField] private bool _isLightableState;
    [SerializeField] private bool _isLightWorking;
    public bool IsLightWorking { get => _isLightWorking; }
    [SerializeField] private bool _isLightButtonPressable = true;
    public bool IsLightButtonPressable { get => _isLightButtonPressable; }
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _maxAngle;
    [SerializeField] private float _curAngle;
    [SerializeField] private float _lightAngleValue;

    public float PlayerDir { get => this.transform.localScale.x; }

    private PlayerBehaviour _player;

    private void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
    }
    void Update()
    {
        InputState inputState = InputManager.Instance.State;

        _isLightableState = _player.CurrentStateIs<IdleState>() || _player.CurrentStateIs<RunState>();

        // Auto Light Source Off
        if (!_isLightableState && _isLightWorking)
        {
            TurnOffLight();
        }

        // Light Source ON / OFF
        if (_isLightableState)
        {
            if (inputState.LightKey.KeyDown && _isLightButtonPressable)
            {
                if (!_isLightWorking)
                    _player.Animator.SetTrigger("TurnOnLight");
                else
                    _player.Animator.SetTrigger("TurnOffLight");
            }
        }

        if (_isLightWorking)
        {
            _curAngle += _rotateSpeed * inputState.Vertical * Time.deltaTime;
            _curAngle = Mathf.Clamp(_curAngle, -_maxAngle, _maxAngle);

            // -35 ~ 35¸¦ 0 ~ 1·Î Á¤±ÔÈ­
            _lightAngleValue = (_curAngle + _maxAngle) / (2 * _maxAngle);
            _player.Animator.SetFloat("LightAngleValue", _lightAngleValue);
        }
    }

    public void TurnOnLight()
    {
        if (_isLightWorking)
            return;

        _isLightWorking = true;
        _player.Animator.SetBool("IsLightWorking", _isLightWorking);

        // ºûÀ» ÄÒ´Ù
        _light.SetActive(true);
        _player.SoundList.PlaySFX("SE_LightSkill");
    }

    public void TurnOffLight()
    {
        if (!_isLightWorking)
            return;

        _isLightWorking = false;
        _player.Animator.SetBool("IsLightWorking", _isLightWorking);

        // ÃÊ±âÈ­
        _curAngle = 0f;
        _lightAngleValue = (_curAngle + _maxAngle) / (2 * _maxAngle);
        _player.Animator.SetFloat("LightAngleValue", _lightAngleValue);

        // ºûÀ» ²ö´Ù
        _light.SetActive(false);
    }

    public void LightButtonPressable()
    {
        _isLightButtonPressable = true;
    }
    public void LightButtonDisPressable()
    {
        _isLightButtonPressable = false;
    }
}
