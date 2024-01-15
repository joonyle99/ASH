using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class LightController : MonoBehaviour
{
    [SerializeField] private GameObject _light;

    [Space]

    [SerializeField] private bool _isLightableState;
    [SerializeField] private bool _isLightWorking;
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

        // Auto Turn Off Light
        if (!_isLightableState && _isLightWorking)
        {
            TurnOffLight();
            _isLightWorking = false;

            return;
        }

        // Light Source ON / OFF
        if (_isLightableState)
        {
            if (inputState.LightKey.KeyDown)
                _isLightWorking = !_isLightWorking;
        }

        // -35 ~ 35¸¦ 0 ~ 1·Î Á¤±ÔÈ­
        _lightAngleValue = (_curAngle + _maxAngle) / (2 * _maxAngle);

        // Animator Parameter
        _player.Animator.SetBool("IsLightWorking", _isLightWorking);
        _player.Animator.SetFloat("LightAngleValue", _lightAngleValue);

        if (_isLightWorking)
        {
            _curAngle += (PlayerDir > 0f ? _rotateSpeed : -_rotateSpeed) * inputState.Vertical * Time.deltaTime;
            _curAngle = Mathf.Clamp(_curAngle, -_maxAngle, _maxAngle);
        }
    }

    public void TurnOnLight()
    {
        // ºûÀ» ÄÒ´Ù
        _light.SetActive(true);
    }

    public void TurnOffLight()
    {
        // ºûÀ» ²ö´Ù
        _light.SetActive(false);
    }
}
