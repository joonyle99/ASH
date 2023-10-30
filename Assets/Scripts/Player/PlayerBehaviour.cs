using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBehaviour : StateMachineBase
{
    [Header("Ground / Wall Checker Setting")]

    [Space]

    [SerializeField] LayerMask _groundLayer;
    [SerializeField] Transform _groundCheckTrans;
    [SerializeField] LayerMask _wallLayer;
    [SerializeField] Transform _wallCheckTrans;

    [Header("Check Distance")]

    [Space]

    [SerializeField] float _groundCheckRadius = 0.35f;
    // [Range(0f, 5f)] [SerializeField] float _groundCheckDistance;
    [Range(0f, 30f)] [SerializeField] float _diveCheckDistance;
    // [Range(0f, 5f)] [SerializeField] float _wallCheckDistance;
    [SerializeField] Vector2 _wallCheckSzie = new Vector2(0.3f, 2f);

    [Header("Dive Settings")]

    [Space]

    [Range(0f, 10f)] [SerializeField] float _diveThreshholdHeight;

    [Header("Ability Settings")]

    [Space]

    [Range(0, 200)] [SerializeField] int _maxHp;

    [SerializeField] int _curHp;

    [Header("Player Settings")]

    [Space]

    // Effect
    [SerializeField] ParticleSystem respawnEffect;
    [SerializeField] float _reviveFadeInDuration;
    [SerializeField] SkinnedMeshRenderer _capeRenderer;

    // Health UI
    [SerializeField] HealthPanelUI _healthPanelUI;

    // Controller
    PlayerJumpController _jumpController;
    PlayerAttackController _attackController;
    InteractionController _interactionController;

    PlayerInputPreprocessor _inputPreprocessor;

    // State
    DashState _dashState;
    DiveState _diveState;
    ShootingState _shootingState;

    // temp velocity
    public Vector3 tempVelocity;

    // IsMove
    private bool IsMove
    {
        get
        {
            return Mathf.Abs(this.Rigidbody.velocity.x) > 0.1f;
        }
    }

    #region Properties

    public bool IsGrounded { get; set; }
    public bool IsTouchedWall { get; set; }

    public bool CanBasicAttack { get { return StateIs<IdleState>() || StateIs<RunState>() || StateIs<InAirState>(); } }
    public bool CanHealing { get { return StateIs<IdleState>(); } }
    public bool CanShootingAttack { get { return StateIs<IdleState>(); } }

    public bool CanDash { get; set; }

    public RaycastHit2D GroundHit { get; set; }
    public RaycastHit2D DiveHit { get; set; }
    public RaycastHit2D WallHit { get; set; }

    public InputState RawInputs { get { return InputManager.Instance.GetState(); } }
    public InputState SmoothedInputs { get { return _inputPreprocessor.SmoothedInputs; } }
    public InteractionController InteractionController { get { return _interactionController; } }   // InputManager.Instance.GetState() 와 동일

    public int RecentDir { get; set; }
    public Vector2 PlayerLookDir { get { return new Vector2(RecentDir, 0); } }

    public bool IsWallJump { get; set; }
    public float GroundDistance { get; set; }
    public float DiveThreshholdHeight
    {
        get { return _diveThreshholdHeight; }
        private set { _diveThreshholdHeight = value; }
    }


    public int CurHP
    {
        get { return _curHp; }
        set {
            _curHp = value;
            if (_curHp < 0)
                _curHp = 0;

            // _healthPanelUI.Life = value;
        }
    }

    public SkinnedMeshRenderer CapeRenderer { get { return _capeRenderer; } }

    #endregion

    private void Awake()
    {
        // Controller
        _jumpController = GetComponent<PlayerJumpController>();
        _attackController = GetComponent<PlayerAttackController>();
        _interactionController = GetComponent<InteractionController>();

        // InputPreProcessor
        _inputPreprocessor = GetComponent<PlayerInputPreprocessor>();

        // State
        _dashState = GetComponent<DashState>();
        _diveState = GetComponent<DiveState>();
        _shootingState = GetComponent<ShootingState>();
    }
    protected override void Start()
    {
        base.Start();

        // TEMP!!
        SoundManager.Instance.PlayCommonBGM("Exploration1", 0.3f);

        InputManager.Instance.JumpPressedEvent += _jumpController.OnJumpPressed; //TODO : subscribe
        InputManager.Instance.BasicAttackPressedEvent += OnBasicAttackPressed; //TODO : subscribe
        InputManager.Instance.HealingPressedEvent += OnHealingPressed; //TODO : subscribe
        InputManager.Instance.ShootingAttackPressedEvent += OnShootingAttackPressed; //TODO : subscribe

        // Init Value
        CurHP = _maxHp;
        RecentDir = 1;
    }

    /// <summary>
    /// 리스폰 되었을때 초기화
    /// </summary>
    private void OnEnable()
    {
        if(StateIs<DieState>())
            StartCoroutine(Alive());
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.JumpPressedEvent -= _jumpController.OnJumpPressed; //TODO : unsubscribe
            InputManager.Instance.BasicAttackPressedEvent -= OnBasicAttackPressed; //TODO : unsubscribe
            InputManager.Instance.HealingPressedEvent -= OnHealingPressed; //TODO : unsubscribe
            InputManager.Instance.ShootingAttackPressedEvent -= OnShootingAttackPressed; //TODO : unsubscribe
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

        // temp velocity
        tempVelocity = this.Rigidbody.velocity;

        #endregion

        #region Basic Behavior

        // Player Flip
        if (StateIs<RunState>() || StateIs<InAirState>())
        {
            if (Mathf.RoundToInt(RawInputs.Movement.x) != 0 && RecentDir != Mathf.RoundToInt(RawInputs.Movement.x))
                UpdateImageFlip();
        }

        // In Air State
        if (!IsGrounded && !StateIs<InAirState>())
        {
            if (!StateIs<WallState>() && !StateIs<DashState>() && !StateIs<DiveState>() && !StateIs<ShootingState>() && !StateIs<HurtState>() && !StateIs<DieState>())
                ChangeState<InAirState>();
        }

        #endregion

        #region Check Ground & Wall

        // Check Ground
        // TODO : BoxCast()로 변경하기. 변경 시 플레이어가 플랫폼 끝부분에 끼는 일이 없다.
        // GroundHit = Physics2D.Raycast(_groundCheckTrans.position, Vector2.down, _groundCheckDistance, _groundLayer);
        // GroundHit = Physics2D.CapsuleCast(_groundCheckTrans.position, _groundCheckSize, CapsuleDirection2D.Vertical, 0f, Vector2.down, );
        GroundHit = Physics2D.CircleCast(_groundCheckTrans.position, _groundCheckRadius, Vector2.down, 0f, _groundLayer);

        if (GroundHit)
            IsGrounded = true;
        else
            IsGrounded = false;

        // Check Wall
        // WallHit = Physics2D.Raycast(_wallCheckTrans.position, Vector2.right * RecentDir, _wallCheckDistance, _wallLayer);
        WallHit = Physics2D.BoxCast(_wallCheckTrans.position, _wallCheckSzie, 0f, Vector2.right * RecentDir, 0f, _wallLayer);

        if (WallHit)
            IsTouchedWall = true;
        else
            IsTouchedWall = false;

        // Check Dive Hit
        DiveHit = Physics2D.Raycast(_groundCheckTrans.position, Vector2.down, _diveCheckDistance, _groundLayer);

        // Ground Distance
        GroundDistance = _groundCheckTrans.position.y - DiveHit.point.y;

        #endregion

        #region Skill CoolTime

        // Dash CoolTime
        if (!_dashState.IsDashing)
        {
            if (Time.time >= _dashState.TimeEndedDash + _dashState.CoolTime)
            {
                if (IsGrounded || StateIs<WallState>())
                    CanDash = true;
            }
        }

        // Dive CoolTime

        // Shooting CoolTime

        #endregion

        // 임시 Healing State
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (StateIs<IdleState>())
                ChangeState<HealingState>();
        }
    }

    /// <summary>
    /// 플레이어 좌우 방향 전환
    /// </summary>
    private void UpdateImageFlip()
    {
        RecentDir = (int)RawInputs.Movement.x;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * RecentDir, transform.localScale.y, transform.localScale.z);
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

    public void OnHitByBatSkill(BatSkillParticle particle, int damage, Vector2 vec)
    {
        Debug.Log("박쥐 점액에 맞음");
        OnHit(damage, vec);
    }

    /// <summary>
    /// 물 웅덩이에 빠지는 함수
    /// </summary>
    /// <param name="damage"></param>
    public void OnHitbyPuddle(float damage)
    {
        Debug.Log("물 웅덩이에 닿음 ");
        //애니메이션, 체력 닳기 등 하면 됨.
        //애니메이션 종료 후 spawnpoint에서 생성
        if (CurHP == 1)
        {
            CurHP = _maxHp;
        }
        else
        {
            CurHP -= 1;
        }
        InstantRespawn();
    }
    public void OnHitByPhysicalObject(float damage, Collision2D collision)
    {
       //TODO
        Debug.Log(damage + " 대미지 입음");
    }
    public void TriggerInstantRespawn(float damage)
    {
        //TEMP
        if (CurHP == 1)
        {
            CurHP = _maxHp;
        }
        else
        {
            CurHP -= 1;
        }
        InstantRespawn();
    }
    void InstantRespawn()
    {
        //TEMP
        gameObject.SetActive(false);
        SceneContext.Current.InstantRespawn();
    }

    /// <summary>
    /// 피격 함수
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="vec"></param>
    public void OnHit(int damage, Vector2 vec)
    {
        CurHP -= damage;
        Rigidbody.velocity = vec;
        RecentDir = (int)Mathf.Sign(-vec.x);
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * RecentDir, transform.localScale.y, transform.localScale.z);
        ChangeState<HurtState>();
    }

    /// <summary>
    /// 플레이어 부활 함수
    /// </summary>
    /// <returns></returns>
    public IEnumerator Alive()
    {
        Debug.Log("부활 !!");

        // 초기 설정
        ChangeState<IdleState>();
        CurHP = _maxHp;
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

    public void PlaySound_SE_Run()
    {
        GetComponent<SoundList>().PlaySFX("SE_Run");
    }

    public void PlaySound_SE_Jump_01()
    {
        GetComponent<SoundList>().PlaySFX("SE_Jump_01");
    }

    public void PlaySound_SE_Jump_02()
    {
        GetComponent<SoundList>().PlaySFX("SE_Jump_02");
    }

    public void PlaySound_SE_DoubleJump()
    {
        GetComponent<SoundList>().PlaySFX("SE_DoubleJump");
    }

    public void PlaySound_SE_Attack()
    {
        GetComponent<SoundList>().PlaySFX("SE_Attack");
    }

    public void PlayerSound_SE_Dash()
    {
        GetComponent<SoundList>().PlaySFX("SE_Dash");
    }

    public void PlaySound_SE_DesolateDive_01()
    {
        GetComponent<SoundList>().PlaySFX("SE_DesolateDive_01");
    }

    public void PlaySound_SE_DesolateDive_02()
    {
        GetComponent<SoundList>().PlaySFX("SE_DesolateDive_02");
    }

    public void PlaySound_SE_Shooting_01()
    {
        GetComponent<SoundList>().PlaySFX("SE_Shooting_01");
    }

    public void PlaySound_SE_Shooting_02()
    {
        GetComponent<SoundList>().PlaySFX("SE_Shooting_02");
    }

    public void PlaySound_SE_Hurt_01()
    {
        GetComponent<SoundList>().PlaySFX("SE_Hurt_01");
    }

    public void PlaySound_SE_Hurt_02()
    {
        GetComponent<SoundList>().PlaySFX("SE_Hurt_02");
    }

    public void PlaySound_SE_Die_01()
    {
        GetComponent<SoundList>().PlaySFX("SE_Die_01(long)");
    }

    public void PlaySound_SE_Die_02()
    {
        GetComponent<SoundList>().PlaySFX("SE_Die_02");
    }

    public void PlaySound_SE_Healing_01()
    {
        GetComponent<SoundList>().PlaySFX("SE_Healing_01");
    }

    public void PlaySound_SE_Healing_02()
    {
        GetComponent<SoundList>().PlaySFX("SE_Healing_02");
    }

    /// <summary>
    /// Ground, Wall, Dive Raycast 그리기
    /// </summary>
    // private void OnDrawGizmosSelected()
    private void OnDrawGizmos()
    {
        // Draw Ground Check
        Gizmos.color = Color.blue;
        // Gizmos.DrawLine(_groundCheckTrans.position, _groundCheckTrans.position + Vector3.down * _groundCheckDistance);
        // Gizmos.DrawWireSphere();
        Gizmos.DrawWireSphere(_groundCheckTrans.position, _groundCheckRadius);

        // Draw Wall Check
        Gizmos.color = Color.red;
        // Gizmos.DrawLine(_wallCheckTrans.position, _wallCheckTrans.position + Vector3.right * _wallCheckDistance * RecentDir);
        Gizmos.DrawWireCube(_wallCheckTrans.position, _wallCheckSzie);

        // Draw Dive Check
        Gizmos.color = Color.white;
        Gizmos.DrawLine(_groundCheckTrans.position + new Vector3(0.1f, 0),
            _groundCheckTrans.position + new Vector3(0.1f, -_diveCheckDistance));
    }
}