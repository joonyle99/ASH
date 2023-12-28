using System.Collections;
using UnityEngine;

/// <summary>
/// 몬스터의 기본 행동을 정의
/// </summary>
public abstract class MonsterBehavior : StateMachineBase
{
    #region Attribute

    [Header("Monster Behavior")] [Space] [SerializeField]
    private MonsterData _monsterData;

    [SerializeField] private Collider2D _collider2D;
    public Collider2D MonsterCollider2D { get => _collider2D; }

    // 고유 식별 번호 ID
    [SerializeField] private int _id;
    public int ID
    {
        get => _id;
        protected set => _id = value;
    }

    // 몬스터 이름
    [SerializeField] private string _monsterName;
    public string MonsterName
    {
        get => _monsterName;
        protected set => _monsterName = value;
    }

    // 최대 체력
    [SerializeField] private int _maxHp;
    public int MaxHp
    {
        get => _maxHp;
        protected set => _maxHp = value;
    }

    // 현재 체력
    [SerializeField] private int _curHp;
    public int CurHp
    {
        get => _curHp;
        protected set => _curHp = value;
    }

    // 이동속도
    [SerializeField] private float _moveSpeed;
    public float MoveSpeed
    {
        get => _moveSpeed;
        protected set => _moveSpeed = value;
    }

    [Space]

    // 그 외 속성들
    [SerializeField] private bool _isDead;
    public bool Dead
    {
        get => _isDead;
        protected set => _isDead = value;
    }

    [SerializeField] private bool _isInAir;
    public bool InAir
    {
        get => _isInAir;
        protected set => _isInAir = value;
    }

    [SerializeField] private bool _isReturn;
    public bool IsReturn
    {
        get => _isReturn;
        protected set => _isReturn = value;
    }

    [Space]

    // 추가 프로퍼티
    [SerializeField] private MonsterDefine.SIZE _monsterSize;
    public MonsterDefine.SIZE MonsterSize // 몬스터 크기 구분
    {
        get => _monsterSize;
        protected set => _monsterSize = value;
    }

    [SerializeField] private MonsterDefine.TYPE _monsterType;
    public MonsterDefine.TYPE MonsterType // 몬스터 종류
    {
        get => _monsterType;
        protected set => _monsterType = value;
    }

    [SerializeField] private MonsterDefine.ACTION _actionType;
    public MonsterDefine.ACTION ActionType // 몬스터 활동 종류
    {
        get => _actionType;
        protected set => _actionType = value;
    }

    [SerializeField] private MonsterDefine.RESPONE _responseType;
    public MonsterDefine.RESPONE ResponseType // 리젠 방식 구분
    {
        get => _responseType;
        protected set => _responseType = value;
    }

    [SerializeField] private MonsterDefine.AGGRESSIVE _aggressiveType;
    public MonsterDefine.AGGRESSIVE AggressiveType // 선공 여부
    {
        get => _aggressiveType;
        protected set => _aggressiveType = value;
    }

    [SerializeField] private MonsterDefine.CHASE _chaseType;
    public MonsterDefine.CHASE ChaseType // 추적 방식 구분
    {
        get => _chaseType;
        protected set => _chaseType = value;
    }

    [SerializeField] private MonsterDefine.RUNAWAY _runawayType;
    public MonsterDefine.RUNAWAY RunawayType // 도망 여부
    {
        get => _runawayType;
        protected set => _runawayType = value;
    }

    [Space]

    [SerializeField] private float _elapsedFadeOutTime;
    [SerializeField] private float _targetFadeOutTime = 3f;

    private bool _isDieTrigger;

    #endregion

    #region Function

    /// <summary>
    /// 컴포넌트로 붙어있는 스크립트가 실행될 때 한 번 실행된다.
    /// 활성화 되어있지 않아도 실행된다.
    /// </summary>
    protected virtual void Awake()
    {
        // Component
        _collider2D = this.GetComponent<Collider2D>();

        if (!_collider2D)
            Debug.Log("Monster has not Collider2D Component");

        // 몬스터 속성 설정
        SetUp();
    }

    protected override void Start()
    {
        base.Start();

        // 박쥐의 현재 체력
        CurHp = MaxHp;
    }

    protected override void Update()
    {
        base.Update();

        if (CurHp == 0)
            Dead = true;

        if (Dead && !_isDieTrigger)
        {
            Die();
            _isDieTrigger = true;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    // 몬스터 속성 세팅
    public virtual void SetUp()
    {
        // 몬스터의 ID 설정
        ID = _monsterData.ID;

        // 몬스터의 이름 설정
        MonsterName = _monsterData.MonsterName;

        // 몬스터의 타입 설정
        MonsterType = _monsterData.MonsterType;

        // 몬스터의 최대 체력
        MaxHp = _monsterData.MaxHp;

        // 몬스터의 이동속도 설정
        MoveSpeed = _monsterData.MoveSpeed;

        // 몬스터의 크기 타입
        MonsterSize = _monsterData.MonsterSize;

        // 몬스터의 활동 종류 타입
        ActionType = _monsterData.ActionType;

        // 몬스터의 리젠 타입
        ResponseType = _monsterData.ResponseType;

        // 몬스터의 선공 타입
        AggressiveType = _monsterData.AggressiveType;

        // 몬스터의 추적 타입
        ChaseType = _monsterData.ChaseType;

        // 몬스터의 도망 타입
        RunawayType = _monsterData.RunawayType;
    }

    // 데미지 피격
    public virtual void OnDamage(int damage)
    {
        Debug.Log("MonsterBehavior의 OnDamage()");
        CurHp -= damage;

        if (CurHp <= 0)
            CurHp = 0;
    }

    // 넉백
    public virtual void KnockBack(Vector2 force)
    {
        Rigidbody.AddForce(force, ForceMode2D.Impulse);
    }

    // 사망
    public virtual void Die()   // 최하위 자식의 Die()을 호출한다.
                                // Polymorphism (다형성)
    {
        Debug.Log("MonsterBehavior의 Die()");

        // 충돌 비활성화
        if (_collider2D)
            _collider2D.enabled = false;

        // 사라지기 시작
        StartCoroutine(FadeOutObject());
    }

    public IEnumerator FadeOutObject()
    {
        // 자식 오브젝트의 모든 SpriteRenderer를 가져온다
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        // 초기 알파값 저장
        float[] startAlphaArray = new float[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            startAlphaArray[i] = spriteRenderers[i].color.a;

        // 모든 렌더 컴포넌트를 돌면서 Fade Out
        while (_elapsedFadeOutTime < _targetFadeOutTime)
        {
            _elapsedFadeOutTime += Time.deltaTime;
            float normalizedTime = _elapsedFadeOutTime / 2;

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                // 현재 스프라이트 렌더러의 알파값을 변경
                Color targetColor = spriteRenderers[i].color;
                targetColor.a = Mathf.Lerp(startAlphaArray[i], 0f, normalizedTime);
                spriteRenderers[i].color = targetColor;
            }

            yield return null;
        }

        // 오브젝트 삭제
        Destroy(gameObject);

        yield return null;
    }

    #endregion
}
