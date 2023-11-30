using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerBehaviour : StateMachineBase
{
    [Header("Ground Check")]
    [Space]

    [SerializeField] LayerMask _groundLayer;
    [SerializeField] Transform _groundCheckTrans;
    [SerializeField] float _groundCheckRadius;

    [Header("Wall Check")]
    [Space]

    [SerializeField] LayerMask _wallLayer;
    [SerializeField] Transform _wallCheckTrans;
    [SerializeField] float _wallCheckDistance;

    [Header("Dive Check")]
    [Space]

    [SerializeField] float _diveCheckDistance;
    [SerializeField] float _diveThreshholdHeight;

    [Header("Player Settings")]
    [Space]

    [SerializeField] int _maxHp;
    [SerializeField] int _curHp;

    [Header("Viewr")]
    [Space]

    [SerializeField] Collider2D _groundHitCollider;
    [SerializeField] Collider2D _wallHitCollider;
    [SerializeField] Collider2D _DiveHitCollider;
    [SerializeField] Collider2D _mainCollider;
    [SerializeField] SkinnedMeshRenderer _capeRenderer;

    // Controller
    PlayerJumpController _jumpController;
    PlayerAttackController _attackController;
    InteractionController _interactionController;

    // State
    DashState _dashState;

    //Joint for interactable
    Joint2D _joint;

    // Sound List
    SoundList _soundList;

    // Padding Vector
    readonly Vector3 _paddingVec = new Vector3(0.1f, 0f, 0f);

    #region Properties

    public bool IsGrounded { get; private set; }
    public bool IsTouchedWall { get; private set; }
    public bool CanBasicAttack { get { return StateIs<IdleState>() || StateIs<RunState>() || StateIs<InAirState>(); } }
    public bool CanShootingAttack { get { return StateIs<IdleState>(); } }
    public bool CanDash { get; set; }

    public int RecentDir { get; set; }
    public Vector2 PlayerLookDir2D { get { return new Vector2(RecentDir, 0f); } }
    public Vector3 PlayerLookDir3D { get { return new Vector3(RecentDir, 0f, 0f); } }
    public bool IsLookForceSync { get { return Math.Abs(PlayerLookDir2D.x - RawInputs.Horizontal) < 0.1f; } }
    public bool IsMoveYKey { get { return Math.Abs(Mathf.RoundToInt(RawInputs.Movement.y)) > 0f; } }
    public bool IsMoveUpKey { get { return Mathf.RoundToInt(RawInputs.Movement.y) > 0f; } }
    public bool IsMoveDownKey { get { return Mathf.RoundToInt(RawInputs.Movement.y) < 0f; } }
    public bool IsMove { get { return Mathf.Abs(this.Rigidbody.velocity.x) > 0.1f; } }
    public bool IsWallJump { get; set; }
    public bool IsInteractable { get { return StateIs<IdleState>() || StateIs<RunState>(); } }
    public float GroundDistance { get; set; }
    public float DiveThreshholdHeight { get { return _diveThreshholdHeight; } private set { _diveThreshholdHeight = value; } }
    public int CurHp { get { return _curHp; } set { _curHp = value; } }

    public Collider2D MainCollider { get { return _mainCollider; } }
    public RaycastHit2D GroundHit { get; set; }
    public RaycastHit2D WallHit { get; set; }
    public RaycastHit2D DiveHit { get; set; }

    public InputState RawInputs { get { return InputManager.Instance.GetState(); } }
    public InteractionController InteractionController { get { return _interactionController; } }   // InputManager.Instance와 동일
    public SkinnedMeshRenderer CapeRenderer { get { return _capeRenderer; } }

    #endregion

    private void Awake()
    {
        // Collider
        _mainCollider = GetComponent<Collider2D>();

        // Controller
        _jumpController = GetComponent<PlayerJumpController>();
        _attackController = GetComponent<PlayerAttackController>();
        _interactionController = GetComponent<InteractionController>();

        // State
        _dashState = GetComponent<DashState>();

        // SoundList
        _soundList = GetComponent<SoundList>();
    }

    protected override void Start()
    {
        base.Start();

        // 배경 BGM 출력
        SoundManager.Instance.PlayCommonBGM("Exploration1", 0.3f);

        InputManager.Instance.JumpPressedEvent += _jumpController.OnJumpPressed; //TODO : subscribe

        /*
        InputManager.Instance.BasicAttackPressedEvent += OnBasicAttackPressed; //TODO : subscribe
        InputManager.Instance.ShootingAttackPressedEvent += OnShootingAttackPressed; //TODO : subscribe
        */
    }

    private void OnEnable()
    {
        // 초기화
        CurHp = _maxHp;
        RecentDir = 1;
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.JumpPressedEvent -= _jumpController.OnJumpPressed; //TODO : unsubscribe

            /*
            InputManager.Instance.BasicAttackPressedEvent -= OnBasicAttackPressed; //TODO : unsubscribe
            InputManager.Instance.ShootingAttackPressedEvent -= OnShootingAttackPressed; //TODO : unsubscribe
            */
        }
    }

    protected override void Update()
    {
        base.Update();

        #region Animaotr Parameter

        Animator.SetBool("IsGround", IsGrounded);
        Animator.SetFloat("AirSpeedY", Rigidbody.velocity.y);
        Animator.SetFloat("GroundDistance", GroundDistance);
        Animator.SetBool("IsMove", IsMove);
        Animator.SetFloat("InputHorizontal", RawInputs.Horizontal);
        Animator.SetFloat("PlayerLookDirX", PlayerLookDir2D.x);
        Animator.SetBool("IsLookForceSync", IsLookForceSync);

        #endregion

        #region Basic Behavior

        // Player Flip
        UpdateImageFlip();

        // Change In Air State
        ChangeInAirState();

        #endregion

        #region Check Ground & Wall

        // Check Ground
        GroundHit = Physics2D.CircleCast(_groundCheckTrans.position, _groundCheckRadius, Vector2.down, 0f, _groundLayer);
        IsGrounded = GroundHit.collider != null;
        _groundHitCollider = GroundHit.collider;

        // Check Wall
        // WallHit = Physics2D.BoxCast(_wallCheckTrans.position, _wallCheckSize, 0f, PlayerLookDir2D, 0f, _wallLayer);
        WallHit = Physics2D.Raycast(_wallCheckTrans.position, PlayerLookDir2D, _wallCheckDistance, _wallLayer);
        IsTouchedWall = WallHit.collider != null;
        _wallHitCollider = WallHit.collider;

        // Check Dive Hit
        DiveHit = Physics2D.Raycast(_groundCheckTrans.position, Vector2.down, _diveCheckDistance, _groundLayer);
        GroundDistance = _groundCheckTrans.position.y - DiveHit.point.y;
        _DiveHitCollider = DiveHit.collider;

        #endregion

        #region Skill CoolTime

        // Dash CoolTime
        CoolTime_Dash();

        #endregion
    }

    private void UpdateImageFlip()
    {
        if (StateIs<RunState>() || StateIs<InAirState>())
        {
            if (Mathf.RoundToInt(RawInputs.Movement.x) != 0 && RecentDir != Mathf.RoundToInt(RawInputs.Movement.x))
            {
                RecentDir = (int)RawInputs.Movement.x;
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * RecentDir, transform.localScale.y, transform.localScale.z);
            }
        }
    }
    private void ChangeInAirState()
    {
        if (!IsGrounded && !StateIs<InAirState>())
        {
            if (!StateIs<WallState>() && !StateIs<DashState>() && !StateIs<DiveState>() && !StateIs<ShootingState>() && !StateIs<HurtState>() && !StateIs<DieState>())
                ChangeState<InAirState>();
        }
    }

    public void AddJoint<T>(Rigidbody2D bodyToAttach, float breakForce) where T : Joint2D
    {
        _joint = gameObject.AddComponent<HingeJoint2D>();
        _joint.connectedBody = bodyToAttach;
        _joint.enableCollision = true;
        _joint.breakForce = breakForce;
    }
    public void RemoveJoint()
    {
        Destroy(_joint);
    }

    void OnBasicAttackPressed()
    {
        if (CanBasicAttack)
            _attackController.CastBasicAttack();
    }
    void OnHealingPressed()
    {

    }
    void OnShootingAttackPressed()
    {
        if (CanShootingAttack)
            _attackController.CastShootingAttack();
    }

    void CoolTime_Dash()
    {
        if (!_dashState.IsDashing)
        {
            if (Time.time >= _dashState.TimeEndedDash + _dashState.CoolTime)
            {
                if (IsGrounded || StateIs<WallState>())
                    CanDash = true;
            }
        }
    }

    public void OnHitbyPuddle(float damage)
    {
        Debug.Log("물 웅덩이에 닿음 ");
        //애니메이션, 체력 닳기 등 하면 됨.
        //애니메이션 종료 후 spawnpoint에서 생성
        if (CurHp == 1)
        {
            CurHp = _maxHp;
        }
        else
        {
            CurHp -= 1;
        }
        InstantRespawn();
    }
    public void OnHitByPhysicalObject(float damage, Rigidbody2D other)
    {
        //TODO
        Debug.Log(damage + " 대미지 입음");
    }
    public void TriggerInstantRespawn(float damage)
    {
        //TEMP
        if (CurHp == 1)
        {
            CurHp = _maxHp;
        }
        else
        {
            CurHp -= 1;
        }
        InstantRespawn();
    }
    void InstantRespawn()
    {
        //TEMP
        gameObject.SetActive(false);
        SceneContext.Current.InstantRespawn();
    }

    public void Interact()
    {
        ChangeState<InteractionState>();
    }

    /*
    public void OnHitByBatSkill(BatSkillParticle particle, int damage, Vector2 vec)
    {
        Debug.Log("박쥐 점액에 맞음");
        OnHit(damage, vec);
    }
    */

    /*
    public void OnHit(int damage, Vector2 vec)
    {
        CurHp -= damage;
        Rigidbody.velocity = vec;
        RecentDir = (int)Mathf.Sign(-vec.x);
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * RecentDir, transform.localScale.y, transform.localScale.z);
        ChangeState<HurtState>();
    }
    */

    /*
    public IEnumerator Alive()
    {
        Debug.Log("부활 !!");

        // 초기 설정
        ChangeState<IdleState>();
        CurHp = _maxHp;
        RecentDir = 1;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * RecentDir, transform.localScale.y, transform.localScale.z);

        // 콜라이더 활성화
        this.GetComponent<Collider2D>().enabled = true;

        // 파티클 생성 & 시작
        ParticleSystem myEffect = Instantiate(respawnEffect, transform.position, Quaternion.identity, transform);
        myEffect.Play();  // 반복되는 이펙트

        // 자식 오브젝트의 모든 렌더 컴포넌트를 가져온다
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(false);

        // 초기 알파값 저장
        float[] startAlphas = new float[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            startAlphas[i] = renderers[i].color.a;

        // 모든 렌더 컴포넌트를 돌면서 Fade In
        float t = 0;
        while (t < _reviveFadeInDuration)
        {
            t += Time.deltaTime;
            float normalizedTime = t / _reviveFadeInDuration;

            for (int i = 0; i < renderers.Length; i++)
            {
                Color color = renderers[i].color;
                color.a = Mathf.Lerp(startAlphas[i], 1f, normalizedTime);
                renderers[i].color = color;
                CapeRenderer.sharedMaterial.SetFloat("_Opacity", normalizedTime);
            }

            yield return null;
        }

        // 파티클 종료 & 파괴
        myEffect.Stop();
        Destroy(myEffect.gameObject);

        yield return null;
    }
    */

    public void PlaySound_SE_Run()
    {
        _soundList.PlaySFX("SE_Run");
    }

    public void PlaySound_SE_Jump_01()
    {
        _soundList.PlaySFX("SE_Jump_01");
    }

    public void PlaySound_SE_Jump_02()
    {
        _soundList.PlaySFX("SE_Jump_02");
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
        _soundList.PlaySFX("SE_Hurt_01");
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

    private void OnDrawGizmosSelected()
    {
        // Draw Ground Check
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_groundCheckTrans.position, _groundCheckRadius);

        // Draw Wall Check
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_wallCheckTrans.position, _wallCheckTrans.position + PlayerLookDir3D * _wallCheckDistance);

        // Draw Dive Check
        Gizmos.color = Color.white;
        Gizmos.DrawLine(_groundCheckTrans.position + _paddingVec,
            _groundCheckTrans.position + _paddingVec + Vector3.down * _diveCheckDistance);
    }
}