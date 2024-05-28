using System.Collections.Generic;
using UnityEngine;

public class RollingStone : InteractableObject
{
    [SerializeField] bool _isMaxClampSpeed = true;
    [SerializeField] float _maxRollSpeed;
    [SerializeField] float _pushPower;

    [SerializeField] bool _stopOnRelease = false;
    [SerializeField] bool _canPull = false;

    [SerializeField] Collider2D _interactionZone;
    [SerializeField] LayerMask _playerMask;

    [SerializeField] ParticleHelper _dustParticle;

    // [SerializeField] SoundList _soundList;
    // [SerializeField] float _pushSoundInterval;

    [SerializeField] AudioSource _rollAudio;
    Rigidbody2D _rigidbody;
    IAttackListener _attackableComponent;

    float _rollAudioTiming = 0f;
    float _rollAudioOriginalVolume;

    float _moveDirection = 0;

    public bool IsBreakable => _attackableComponent == null;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _attackableComponent = GetComponent<IAttackListener>();
        _rollAudioOriginalVolume = _rollAudio.volume;
    }
    protected override void OnObjectInteractionEnter()
    {
        //Player.MovementController.enabled = true;
        _moveDirection = Player.PlayerLookDir2D.x;
    }

    private void Update()
    {
        // contacts count
        Collider2D[] colliderArray = new Collider2D[1];
        var contactsCount = _rigidbody.GetContacts(colliderArray);

        var isStoneMoving = Mathf.Abs(_rigidbody.velocity.x) > 0.4f;
        var isStoneContact = contactsCount > 0;

        var isStonePushed = isStoneMoving && isStoneContact;

        // �÷��̾ ���� ���� �̴� ���
        if (isStonePushed)
        {
            if (!_rollAudio.isPlaying)
            {
                _rollAudio.volume = _rollAudioOriginalVolume;
                _rollAudio.Play();

                _dustParticle.Play();
            }
            else
            {
                // ���� �������� ����
                if (_rollAudioTiming < 1f)
                {
                    _rollAudioTiming = 1f;
                    _rollAudio.volume = _rollAudioOriginalVolume * _rollAudioTiming;
                }
            }
        }
        // �ڿ������� ������ ���ߴ� ���
        else
        {
            if (_rollAudio.isPlaying)
            {
                _rollAudioTiming -= Time.deltaTime;
                _rollAudio.volume = _rollAudioOriginalVolume * _rollAudioTiming;

                if (_rollAudioTiming <= 0f)
                {
                    _rollAudioTiming = 0f;
                    _rollAudio.Stop();

                    _dustParticle.Stop();
                }
            }
        }
    }
    public override void UpdateInteracting()
    {
        List<Collider2D> contacts = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();

        filter.layerMask = _playerMask;
        filter.useLayerMask = true;

        var contactCount = _interactionZone.OverlapCollider(filter, contacts);

        if (IsInteractionKeyUp || !IsPlayerInteractionState ||
            (!_canPull && Player.RawInputs.Movement.x * _moveDirection < 0) ||
            contactCount == 0)
        {
            ExitInteraction();
        }
    }
    public override void FixedUpdateInteracting()
    {
        var isDirSync = Player.RawInputs.Movement.x * _moveDirection > 0.1f;
        var isOppositeDirSync = Player.RawInputs.Movement.x * _moveDirection < -0.1f;
        var isMoveDirNoneSync = Player.RawInputs.Movement.x * _rigidbody.velocity.x < -0.1f;

        if (isDirSync)
        {
            // Debug.Log("�δ�");

            // Rolling Stone�� x�� ������ �ӵ��� �÷��̾ �̴� ������ �ٸ��� �ӵ��� 0���� ������ش�.
            if (isMoveDirNoneSync)
                _rigidbody.velocity = Vector2.zero;

            _rigidbody.AddForce(Player.RawInputs.Movement * _pushPower);

            // �÷��̾ �Բ� �̵��Ѵ�
            Player.Rigidbody.AddForce(Player.RawInputs.Movement * 70f);
        }
        else if (isOppositeDirSync)
        {
            if (_canPull)
            {
                // Debug.Log("����");

                // Rolling Stone�� x�� ������ �ӵ��� �÷��̾ �̴� ������ �ٸ��� �ӵ��� 0���� ������ش�.
                if (isMoveDirNoneSync)
                    _rigidbody.velocity = Vector2.zero;

                _rigidbody.AddForce(Player.RawInputs.Movement * _pushPower * 0.7f);
            }
        }

        if (_isMaxClampSpeed)
        {
            // Debug.Log("���鿡���� �ִ� �ӵ� ���� ��");
            _rigidbody.velocity = Vector2.ClampMagnitude(_rigidbody.velocity, _maxRollSpeed);
        }
    }
    protected override void OnObjectInteractionExit()
    {
        if (_stopOnRelease)
            _rigidbody.velocity *= 0.1f;
        //Player.MovementController.enabled = false;
    }
}
