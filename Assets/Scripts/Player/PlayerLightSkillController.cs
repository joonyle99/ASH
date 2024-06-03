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

            // -35 ~ 35�� 0 ~ 1�� ����ȭ
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

        // ���� �Ҵ�
        _light.SetActive(true);
        _player.SoundList.PlaySFX("SE_LightSkill");
    }

    public void TurnOffLight()
    {
        if (!_isLightWorking)
            return;

        _isLightWorking = false;
        _player.Animator.SetBool("IsLightWorking", _isLightWorking);

        // �ʱ�ȭ
        _curAngle = 0f;
        _lightAngleValue = (_curAngle + _maxAngle) / (2 * _maxAngle);
        _player.Animator.SetFloat("LightAngleValue", _lightAngleValue);

        // ���� ����
        _light.SetActive(false);
    }

    public void LightButtonPressable()
    {
        // �� ��ų ���� �ð� ������ Press �Ұ���
        _isLightButtonPressable = true;
    }
    public void LightButtonDisPressable()
    {
        // �� ��ų ������ ���۵Ǹ� Press �Ұ���
        _isLightButtonPressable = false;
    }
}
