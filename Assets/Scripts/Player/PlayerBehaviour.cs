using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerBehaviour : StateMachineBase, IAttackListener
{
    #region Attribute

    [Header("Ground Check")]
    [Space]

    [SerializeField] LayerMask _groundLayer;
    [SerializeField] Transform _groundCheckTrans;
    [SerializeField] Transform _groundAboveCheckTrans;
    [SerializeField] float _groundCheckRadius;
    [SerializeField] float _groundAboveCheckLength;

    [Header("Climb Check")]
    [Space]

    [SerializeField] LayerMask _climbLayer;
    [SerializeField] Transform _climbCheckTrans;
    [SerializeField] float _climbCheckLength;

    [Header("Player")]
    [Space]

    [SerializeField] int _maxHp;
    [SerializeField] int _curHp;

    [Space]

    [SerializeField] bool _isHurt;
    [SerializeField] bool _isDead;
    [SerializeField] bool _isGodMode;

    [Space]

    [SerializeField] bool _isCanBasicAttack = true;
    [SerializeField] bool _isCanJump = true;
    [SerializeField] bool _isCanDash = true;

    [Header("HeadAim")]
    [Space]

    [SerializeField] Transform _target;
    [SerializeField] float _speed = 5f;
    [SerializeField] float _rightMin = 0.9f;
    [SerializeField] float _rightMax = 1.9f;
    [SerializeField] float _leftMin = 0.4f;
    [SerializeField] float _leftMax = 1.4f;
    [SerializeField] float _cameraSpeed = 1f;
    [SerializeField] float _cameraMin = 0.08f;
    [SerializeField] float _cameraMax = 0.68f;

    [Header("Viewr")]
    [Space]

    [SerializeField] CapsuleCollider2D _bodyCollider;
    [SerializeField] Rigidbody2D _handRigidbody;

    [Header("White Flash")]
    [Space]

    [SerializeField] Material _whiteFlashMaterial;
    [SerializeField] float _godModeTime = 1.5f;
    [SerializeField] float _flashInterval = 0.06f;

    [SerializeField] SpriteRenderer[] _spriteRenderers;
    Material[] _originalMaterials;
    Coroutine _whiteFlashRoutine;

    [Header("Effects")]
    [Space]

    [SerializeField] ParticleHelper _walkDustEmitter;
    [SerializeField] ParticleHelper _walkDirtEmitter;
    [SerializeField] ParticleHelper _landDustEmitter;
    [SerializeField] ParticleHelper _landDirtEmitter;
    [SerializeField] ParticleHelper _dashEffect;
    [SerializeField] ParticleHelper _dashTrailEffect;

    // Controller
    PlayerAttackController _playerAttackController;
    InteractionController _interactionController;
    PlayerMovementController _playerMovementController;
    LightController _lightController;

    // Sound List
    SoundList _soundList;

    #endregion

    #region Properties

    // Can Property
    public bool CanBasicAttack { get { return _isCanBasicAttack && (CurrentStateIs<IdleState>() || CurrentStateIs<RunState>() || CurrentStateIs<InAirState>()) && (_lightController.IsLightButtonPressable && !_lightController.IsLightWorking); } set { _isCanBasicAttack = value; } }
    public bool CanDash { get { return _isCanDash && PersistentDataManager.Get<bool>("Dash"); } set { _isCanDash = value; } }
    public bool CanInteract { get { return CurrentStateIs<IdleState>() || CurrentStateIs<RunState>(); } }

    // Condition Property
    public bool IsGrounded { get { return GroundHit; } }
    public bool IsTouchedWall { get { return ClimbHit; } }
    public bool IsClimbable { get; set; }
    public bool IsClimbJump { get; set; }
    public bool IsHurt { get { return _isHurt; } set { _isHurt = value; } }
    public bool IsDead { get { return _isDead; } set { _isDead = value; } }
    public bool IsGodMode { get { return _isGodMode; } set { _isGodMode = value; } }
    public int CurHp { get { return _curHp; } }

    // Input Property
    public InputState RawInputs { get { return InputManager.Instance.State; } }
    public bool IsMoveXKey { get { return Math.Abs(RawInputs.Movement.x) > 0.01f; } }
    public bool IsMoveRightKey { get { return RawInputs.Movement.x > 0.01f; } }
    public bool IsMoveLeftKey { get { return RawInputs.Movement.x < -0.01f; } }
    public bool IsMoveYKey { get { return Math.Abs(RawInputs.Movement.y) > 0.01f; } }
    public bool IsMoveUpKey { get { return RawInputs.Movement.y > 0.01f; } }
    public bool IsMoveDownKey { get { return RawInputs.Movement.y < -0.01f; } }

    // Direction Property
    public int RecentDir { get; set; }
    public bool IsDirSync { get { return Mathf.Sign(PlayerLookDir2D.x * RawInputs.Movement.x) > 0.01f; } }
    public bool IsOppositeDirSync { get { return Mathf.Sign(PlayerLookDir2D.x * RawInputs.Movement.x) < -0.01f; } }
    public Vector2 PlayerLookDir2D { get { return new Vector2(RecentDir, 0f); } }
    public Vector3 PlayerLookDir3D { get { return new Vector3(RecentDir, 0f, 0f); } }

    // RaycastHit
    public RaycastHit2D GroundHit { get; private set; }
    public RaycastHit2D UpwardGroundHit { get; set; }
    public RaycastHit2D ClimbHit { get; set; }

    // Component
    public PlayerAttackController PlayerAttackController { get { return _playerAttackController; } }
    public InteractionController InteractionController { get { return _interactionController; } }
    public PlayerMovementController PlayerMovementController { get { return _playerMovementController; } }
    public Rigidbody2D HandRigidBody { get { return _handRigidbody; } }
    public CapsuleCollider2D BodyCollider { get { return _bodyCollider; } }
    public SoundList SoundList { get { return _soundList; } }

    // SpriteRenderer / Material
    public SpriteRenderer[] SpriteRenderers { get { return _spriteRenderers; } }
    public Material[] OriginalMaterials => _originalMaterials;

    #endregion

    #region Function

    protected override void Awake()
    {
        // Controller
        _playerAttackController = GetComponent<PlayerAttackController>();
        _interactionController = GetComponent<InteractionController>();
        _playerMovementController = GetComponent<PlayerMovementController>();
        _lightController = GetComponent<LightController>();

        // collider
        _bodyCollider = GetComponent<CapsuleCollider2D>();

        // SoundList
        _soundList = GetComponent<SoundList>();

        // Material for White Flash
        LoadFlashMaterial();
        SaveOriginalMaterial();
    }
    protected override void Start()
    {
        base.Start();

        // init player
        InitPlayer();
    }
    protected override void Update()
    {
        base.Update();

        if (IsDead)
            return;

        #region Input

        if (InputManager.Instance.State.BasicAttackKey.KeyDown)
            OnBasicAttackPressed();

        #endregion

        #region Basic Behavior

        // Player Flip
        UpdateImageFlip();

        // Player Head Aim
        if (CurrentStateIs<IdleState>() || CurrentStateIs<RunState>())
            HeadAimControl();

        // Change In Air State
        ChangeInAirState();

        // Check Dead State
        if (_curHp <= 0)
        {
            ChangeState<DieState>();
            return;
        }

        #endregion

        #region Check Ground & Climb

        // Check Ground
        GroundHit = Physics2D.CircleCast(_groundCheckTrans.position, _groundCheckRadius, Vector2.down, 0f, _groundLayer);

        // Check Upward
        UpwardGroundHit = Physics2D.Raycast(transform.position, Vector2.up, _groundAboveCheckLength, _groundLayer);

        // Check Climb
        ClimbHit = Physics2D.Raycast(_climbCheckTrans.position, PlayerLookDir2D, _climbCheckLength, _climbLayer);
        if (ClimbHit)
        {
            // TODO : 벽의 방향을 localScale로 하면 위험하다
            int wallLookDir = Math.Sign(ClimbHit.transform.localScale.x);
            bool isDirSync = (wallLookDir * RecentDir) > 0;
            IsClimbable = !isDirSync;
        }

        #endregion

        #region Animaotr Parameter

        Animator.SetBool("IsGround", IsGrounded);
        Animator.SetFloat("AirSpeedY", Rigidbody.velocity.y);
        Animator.SetBool("IsDirSync", IsDirSync);

        #endregion
    }

    // basic
    private void InitPlayer()
    {
        _curHp = _maxHp;
        RecentDir = Math.Sign(transform.localScale.x);
    }
    private void UpdateImageFlip()
    {
        if (CurrentStateIs<RunState>() || CurrentStateIs<InAirState>())
        {
            if (IsOppositeDirSync && IsMoveXKey)
            {
                RecentDir = (int)RawInputs.Movement.x;
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * RecentDir, transform.localScale.y, transform.localScale.z);

                // UpdateFlip 할때마다 Target Object의 높이를 맞춰줘야 한다.
                // 0.9 ~ 1.9를 1.4 ~ 0.4에 대응시킨다.
                Vector3 targetVector = _target.localPosition;
                targetVector.y = (_leftMin + _rightMax) - targetVector.y;
                targetVector.y = (RecentDir == 1)
                    ? Mathf.Clamp(targetVector.y, _rightMin, _rightMax)
                    : Mathf.Clamp(targetVector.y, _leftMin, _leftMax);
                _target.localPosition = targetVector;
            }
        }
    }
    private void ChangeInAirState()
    {
        if (!IsGrounded)
        {
            if (CurrentStateIs<IdleState>() || CurrentStateIs<RunState>() || CurrentStateIs<JumpState>())
                ChangeState<InAirState>();
        }
    }

    private void HeadAimControl()
    {
        // 상태에 따라 자연스러운 카메라 및 targetObject 움직임 구현하기

        // target의 높이 변화에 의한 Head Aim 설정
        // multi aim constraint 사용

        // 왼쪽을 바라볼 때 (recentDir == -1)
        // -> down key -> target object move to up (targetMoveDir == 1)
        // -> up key -> target object move to down (targetMoveDir == -1)
        // 오른쪽을 바라볼 때 (recentDir == 1)
        // -> down key -> target object move to down  (targetMoveDir == -1)
        // -> up key -> target object move to up  (targetMoveDir == 1)

        // 플레이어의 바라보는 방향과 키 입력에 따른 target 오브젝트가 움직이는 방향 설정 (기본값 0)
        float targetObjectMoveDir = 0f;

        if (IsMoveUpKey) targetObjectMoveDir = (RecentDir == 1) ? 1f : -1f;
        else if (IsMoveDownKey) targetObjectMoveDir = (RecentDir == 1) ? -1f : 1f;

        float min = (RecentDir == 1) ? _rightMin : _leftMin;
        float max = (RecentDir == 1) ? _rightMax : _leftMax;

        // target 오브젝트가 위 / 아래로 움직이는 경우
        if (Mathf.Abs(targetObjectMoveDir) > 0.001f)
        {
            // 카메라 이동
            float cameraPosY = SceneContext.Current.Camera.OffsetY;
            float cameraMoveDir = Mathf.Sign(RecentDir * targetObjectMoveDir); // (RecentDir * targetObjectMoveDir) == -1 => down key
            cameraPosY += cameraMoveDir * _cameraSpeed * Time.deltaTime;
            cameraPosY = Mathf.Clamp(cameraPosY, _cameraMin, _cameraMax);
            SceneContext.Current.Camera.OffsetY = cameraPosY;

            // target 오브젝트 이동
            Vector3 targetPos = _target.localPosition;
            targetPos.y += targetObjectMoveDir * _speed * Time.deltaTime;
            targetPos.y = Mathf.Clamp(targetPos.y, min, max);
            _target.localPosition = targetPos;
        }
        // target 오브젝트가 제자리로 돌아가는 경우
        else
        {
            // 카메라 이동
            float cameraPosY = SceneContext.Current.Camera.OffsetY;
            float cameraOriginY = (_cameraMin + _cameraMax) / 2f;
            float cameraMoveDir = cameraOriginY - cameraPosY;
            if (Mathf.Abs(cameraMoveDir) > 0.001f)
            {
                cameraPosY += Mathf.Sign(cameraMoveDir) * _cameraSpeed * Time.deltaTime;
                if (cameraMoveDir < 0f) cameraPosY = Mathf.Max(cameraPosY, cameraOriginY);
                else cameraPosY = Mathf.Min(cameraPosY, cameraOriginY);
                SceneContext.Current.Camera.OffsetY = cameraPosY;
            }

            // targetObject 이동
            Vector3 targetPos = _target.localPosition;
            float targetOriginY = (min + max) / 2f;
            if (Mathf.Abs(targetPos.y - targetOriginY) > 0.001f)
            {
                float taretMoveDir = targetOriginY - targetPos.y;
                targetPos.y += MathF.Sign(taretMoveDir) * _speed / 2f * Time.deltaTime;
                if (targetPos.y > targetOriginY) targetPos.y = Mathf.Clamp(targetPos.y, targetOriginY, max);    // target 오브젝트 이동 방향 : 위 -> 아래
                else targetPos.y = Mathf.Clamp(targetPos.y, min, targetOriginY);                                // target 오브젝트 이동 방향 : 아래 -> 위
                _target.localPosition = targetPos;
            }
        }
    }

    // key pressed event
    void OnBasicAttackPressed()
    {
        if (CanBasicAttack)
            _playerAttackController.CastBasicAttack();
    }

    // about hit
    void TakeDamage(float damage)
    {
        _curHp -= (int)damage;
        if (_curHp <= 0)
        {
            _curHp = 0;
            ChangeState<DieState>();
        }
    }
    public void KnockBack(Vector2 forceVector)
    {
        Rigidbody.velocity = Vector2.zero;
        Rigidbody.AddForce(forceVector, ForceMode2D.Impulse);
    }
    public IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        // fail return condition
        if (IsHurt || IsGodMode || IsDead)
            return IAttackListener.AttackResult.Fail;

        PlaySound_SE_Hurt_02();
        StartCoroutine(SlowMotionCoroutine(0.3f));

        TakeDamage((int)attackInfo.Damage);
        // Change Die State
        if (_curHp <= 0)
            return IAttackListener.AttackResult.Success;

        KnockBack(attackInfo.Force);

        // Change Hurt State
        ChangeState<HurtState>();

        return IAttackListener.AttackResult.Success;
    }
    IEnumerator SlowMotionCoroutine(float duration)
    {
        Time.timeScale = 0.3f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
    // flash
    private void LoadFlashMaterial()
    {
        _whiteFlashMaterial =
            Resources.Load<Material>("Materials/WhiteFlashMaterial");
    }
    private void SaveOriginalMaterial()
    {
        _originalMaterials = new Material[_spriteRenderers.Length];

        for (int i = 0; i < _originalMaterials.Length; i++)
            _originalMaterials[i] = _spriteRenderers[i].material;
    }
    public void InitMaterial()
    {
        for (int i = 0; i < _spriteRenderers.Length; i++)
            _spriteRenderers[i].material = _originalMaterials[i];
    }
    public void InitSpriteRendererAlpha()
    {
        foreach (var renderer in SpriteRenderers)
        {
            Color color = renderer.color;
            color.a = 1;
            renderer.color = color;
        }
    }
    private void ChangeMaterial()
    {
        for (int i = 0; i < _originalMaterials.Length; i++)
            _spriteRenderers[i].material = _whiteFlashMaterial;
    }
    private IEnumerator WhiteFlash()
    {
        // turn to white material
        ChangeMaterial();

        while (IsGodMode)
        {
            foreach (var spriteRenderer in _spriteRenderers)
                spriteRenderer.material.SetFloat("_FlashAmount", 0.4f);

            yield return new WaitForSeconds(_flashInterval);

            foreach (var spriteRenderer in _spriteRenderers)
                spriteRenderer.material.SetFloat("_FlashAmount", 0f);

            yield return new WaitForSeconds(_flashInterval);
        }

        // turn to original material
        if (!CurrentStateIs<InstantRespawnState>())
            InitMaterial();
    }
    public void StartWhiteFlash()
    {
        if (this._whiteFlashRoutine != null)
            StopCoroutine(this._whiteFlashRoutine);

        this._whiteFlashRoutine = StartCoroutine(WhiteFlash());
    }

    // god mode
    private IEnumerator GodModeTimer()
    {
        IsGodMode = true;
        yield return new WaitForSeconds(_godModeTime);
        IsGodMode = false;
    }
    public void StartGodModeTimer()
    {
        StartCoroutine(GodModeTimer());
    }

    // respawn
    public void OnRevive()
    {
        _curHp = _maxHp;
    }
    public void TriggerInstantRespawn(float damage)
    {
        TakeDamage(damage);
        if (CurHp > 0)
            InstantRespawn();
    }
    public void InstantRespawn()
    {
        ChangeState<InstantRespawnState>(true);
        SceneContext.Current.InstantRespawn();
    }

    // anim event
    public void FinishState_AnimEvent()
    {
        ChangeState<IdleState>();
    }

    #endregion

    #region Sound

    public void PlaySound_SE_Run()
    {
        _soundList.PlaySFX("SE_Run");
        _walkDustEmitter.Emit(1);
        _walkDirtEmitter.Emit(UnityEngine.Random.Range(0, 3));
    }

    public void PlaySound_SE_Jump_01()
    {
        _soundList.PlaySFX("SE_Jump_01");
    }

    public void PlaySound_SE_Jump_02()
    {
        _soundList.PlaySFX("SE_Jump_02");
        _landDustEmitter.Emit(2);
        _landDirtEmitter.Emit(7);
    }

    public void PlaySound_SE_DoubleJump()
    {
        _soundList.PlaySFX("SE_DoubleJump");
    }

    public void PlaySound_SE_Attack()
    {
        _soundList.PlaySFX("SE_Attack");
    }

    public void PlaySound_SE_Dash()
    {
        _soundList.PlaySFX("SE_Dash");
        _dashEffect.Emit(1);
        _dashTrailEffect.Emit(1);
    }

    public void PlaySound_SE_DesolateDive_01()
    {
        _soundList.PlaySFX("SE_DesolateDive_01");
    }

    public void PlaySound_SE_DesolateDive_02()
    {
        _soundList.PlaySFX("SE_DesolateDive_02");
    }

    public void PlaySound_SE_Shooting_01()
    {
        _soundList.PlaySFX("SE_Shooting_01");
    }

    public void PlaySound_SE_Shooting_02()
    {
        _soundList.PlaySFX("SE_Shooting_02");
    }

    public void PlaySound_SE_Hurt_01()
    {
        // _soundList.PlaySFX("SE_Hurt_01");
    }

    public void PlaySound_SE_Hurt_02()
    {
        _soundList.PlaySFX("SE_Hurt_02");
    }

    public void PlaySound_SE_Die_01()
    {
        _soundList.PlaySFX("SE_Die_01(long)");
    }

    public void PlaySound_SE_Die_02()
    {
        _soundList.PlaySFX("SE_Die_02");
    }

    public void PlaySound_SE_Healing_01()
    {
        _soundList.PlaySFX("SE_Healing_01");
    }

    public void PlaySound_SE_Healing_02()
    {
        _soundList.PlaySFX("SE_Healing_02");
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        // set color
        Gizmos.color = Color.red;

        // Draw Ground Check
        Gizmos.DrawWireSphere(_groundCheckTrans.position, _groundCheckRadius);

        // Draw Ground Above Check
        Gizmos.DrawLine(_groundAboveCheckTrans.position,
            _groundAboveCheckTrans.position + Vector3.up * _groundAboveCheckLength);

        // Draw Wall Check
        Gizmos.DrawLine(_climbCheckTrans.position, _climbCheckTrans.position + PlayerLookDir3D * _climbCheckLength);
    }
}