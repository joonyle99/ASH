using UnityEngine;

public class PlayerLightSkillController : MonoBehaviour
{
    [SerializeField] private GameObject _light;

    [Space]

    [SerializeField] private bool _isLightableState;
    [SerializeField] private bool _isLightWorking;
    public bool IsLightWorking => _isLightWorking;

    [SerializeField] private bool _isLightButtonPressable = true;
    public bool IsLightButtonPressable => _isLightButtonPressable;

    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _maxAngle;
    [SerializeField] private float _curAngle;
    [SerializeField] private float _lightAngleValue;

    public float PlayerDir => this.transform.localScale.x;

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
        if (_isLightWorking && !_isLightableState)
        {
            _player.Animator.SetBool("IsLightWorking", false);
        }

        // Light Source ON / OFF
        if (_isLightableState && PersistentDataManager.GetByGlobal<bool>("_light"))
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

            // -35 ~ 35를 0 ~ 1로 정규화
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

        // 빛을 켠다
        _light.SetActive(true);
        _player.SoundList.PlaySFX("SE_LightSkill");
    }

    public void TurnOffLight()
    {
        if (!_isLightWorking)
            return;

        _isLightWorking = false;
        _player.Animator.SetBool("IsLightWorking", _isLightWorking);

        // 초기화
        _curAngle = 0f;
        _lightAngleValue = (_curAngle + _maxAngle) / (2 * _maxAngle);
        _player.Animator.SetFloat("LightAngleValue", _lightAngleValue);

        // 빛을 끈다
        _light.SetActive(false);
    }

    public void LightButtonPressable()
    {
        // 빛 스킬 시전 시간 동안은 Press 불가능
        _isLightButtonPressable = true;
    }
    public void LightButtonDisPressable()
    {
        // 빛 스킬 시전이 시작되면 Press 불가능
        _isLightButtonPressable = false;
    }
}
