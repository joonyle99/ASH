using System.Collections.Generic;
using UnityEngine;

public class RollingStone : InteractableObject, ISceneContextBuildListener
{
    [SerializeField] private bool _isMaxClampSpeed = true;
    [SerializeField] private float _maxRollSpeed;
    [SerializeField] private float _pushPower;

    [SerializeField] private bool _stopOnRelease = false;
    [SerializeField] private bool _canPull = false;

    [SerializeField] private Collider2D _interactionZone;
    [SerializeField] private LayerMask _playerMask;

    [SerializeField] private ParticleHelper _dustParticle;

    // [SerializeField] SoundList _soundList;
    // [SerializeField] float _pushSoundInterval;

    [SerializeField] private AudioSource _rollAudio;
    private Rigidbody2D _rigidbody;
    private PreserveState _statePreserver;
    private IAttackListener _attackableComponent;

    private float _rollAudioTiming = 0f;
    private float _rollAudioOriginalVolume;

    private float _moveDirection = 0;

    public bool IsBreakable => _attackableComponent == null;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _statePreserver = GetComponent<PreserveState>();
        _attackableComponent = GetComponent<IAttackListener>();

    }
    public void OnSceneContextBuilt()
    {
        if (_statePreserver)
        {
            bool isInteractable = _statePreserver.LoadState("_isInteractable", IsInteractable);
            if (isInteractable)
            {
                IsInteractable = true;
            }
        }

        _rollAudioOriginalVolume = _rollAudio.volume;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_statePreserver)
        {
            _statePreserver.SaveState("_isInteractable", IsInteractable);
        }
    }

    private void Update()
    {
        // contacts count
        Collider2D[] colliderArray = new Collider2D[1];
        var contactsCount = _rigidbody.GetContacts(colliderArray);

        var isStoneMoving = Mathf.Abs(_rigidbody.velocity.x) > 0.4f;
        var isStoneContact = contactsCount > 0;

        var isStonePushed = isStoneMoving && isStoneContact;

        // 플레이어가 돌을 직접 미는 경우
        if (isStonePushed)
        {
            //Debug.Log("============= PUSH =============");

            if (!_rollAudio.isPlaying)
            {
                //Debug.Log("============= PUSH - Start =============");

                _rollAudio.volume = _rollAudioOriginalVolume;
                _rollAudio.Play();

                _dustParticle.Play();
            }
            else
            {
                //Debug.Log("============= PUSH - IsPlaying =============");

                // 기존 음량으로 설정
                if (_rollAudioTiming < 1f)
                {
                    //Debug.Log("============= PUSH - Init Volume =============");

                    _rollAudioTiming = 1f;
                    _rollAudio.volume = _rollAudioOriginalVolume * _rollAudioTiming;
                }
            }
        }
        // 자연스럽게 서서히 멈추는 경우
        else
        {
            //Debug.Log("============= NOT PUSH =============");

            if (_rollAudio.isPlaying)
            {
                //Debug.Log("============= NOT PUSH - IsPlaying =============");

                _rollAudioTiming -= Time.deltaTime;
                _rollAudio.volume = _rollAudioOriginalVolume * _rollAudioTiming;

                if (_rollAudioTiming <= 0f)
                {
                    //Debug.Log("============= NOT PUSH - DONE =============");

                    _rollAudioTiming = 0f;
                    _rollAudio.Stop();

                    _dustParticle.Stop();
                }
            }
        }
    }

    protected override void OnObjectInteractionEnter()
    {
        //Player.MovementController.enabled = true;
        _moveDirection = Player.PlayerLookDir2D.x;
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
            // Debug.Log("민다");

            // Rolling Stone의 x축 방향의 속도와 플레이어가 미는 방향이 다르면 속도를 0으로 만들어준다.
            if (isMoveDirNoneSync)
            {
                _rigidbody.velocity = Vector2.zero;
            }

            _rigidbody.AddForce(Player.RawInputs.Movement * _pushPower);

            // 플레이어도 함께 이동한다
            Player.Rigidbody.AddForce(Player.RawInputs.Movement * 70f);
        }
        else if (isOppositeDirSync)
        {
            if (_canPull)
            {
                // Debug.Log("당긴다");

                // Rolling Stone의 x축 방향의 속도와 플레이어가 미는 방향이 다르면 속도를 0으로 만들어준다.
                if (isMoveDirNoneSync)
                {
                    _rigidbody.velocity = Vector2.zero;
                }

                //뒤에 벽이 없을 때만 힘을 가함, 뒤에 벽이 있을 때는 그 즉시 힘 삭제
                if (!Player.IsWallToBehind)
                    _rigidbody.AddForce(Player.RawInputs.Movement * _pushPower * 0.7f);
                else
                    _rigidbody.velocity = Vector2.zero;
            }
        }

        if (_isMaxClampSpeed)
        {
            // Debug.Log("경사면에서의 최대 속도 조정 중");
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
