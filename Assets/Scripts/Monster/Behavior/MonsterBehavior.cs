using UnityEngine;

#region Enum

public enum SIZE
{
    Small = 0,
    Medium,
    Large
}

public enum TYPE
{
    Normal = 0,
    SemiBoss,
    Boss
}

public enum ACTION_TYPE
{
    Ground = 0,
    Floating,
    Crawl
}

public enum RESPONE
{
    Time = 0,
    Reentry,
    None
}

public enum IS_AGGRESSIVE
{
    Peace = 0,
    SightAggressive,
    TerritoryAggressive,
    AttackAggressive,
    Boss
}

public enum IS_CHASE
{
    Peace = 0,
    Territory,
    AllTerritory
}

public enum IS_RUNAWAY
{
    Aggressive = 0,
    Peace,
    Coward
}

#endregion

/// <summary>
/// 몬스터의 기본 행동을 정의
/// </summary>
public abstract class MonsterBehavior : StateMachineBase
{
    #region Attribute

    [Header("Monster Behavior")]
    [Space]

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


    // 추가 프로퍼티
    public SIZE Size { get; protected set; } // 몬스터 크기 구분
    public TYPE Type { get; protected set; } // 몬스터 종류
    public ACTION_TYPE ActionType { get; protected set; } // 몬스터 활동 종류
    public RESPONE Response { get; protected set; } // 리젠 방식 구분
    public IS_AGGRESSIVE IsAggressive { get; protected set; } // 선공 여부
    public IS_CHASE IsChase { get; protected set; } // 추경 방식 구분
    public IS_RUNAWAY IsRunaway { get; protected set; } // 도망 여부


    #endregion

    #region Function

    protected virtual void Awake()
    {
        // Component
        _collider2D = this.GetComponent<Collider2D>();

        if(!_collider2D)
            Debug.Log("Monster has not Collider2D Component");
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    // 몬스터 속성 세팅
    public virtual void SetUp()
    {
        ID = 0;
        MonsterName = "NULL";
        MaxHp = 100;
        CurHp = 0;
        MoveSpeed = 10;
    }

    // 데미지 피격
    public virtual void OnDamage(int damage)
    {
        CurHp -= damage;

        if (CurHp <= 0)
        {
            CurHp = 0;
            Die();
        }
    }

    // 넉백
    public virtual void KnockBack(Vector2 vec)
    {

    }

    // 사망
    public virtual void Die()
    {
        // 충돌 비활성화
        if(_collider2D)
            _collider2D.enabled = false;

        Dead = true;
    }

    #endregion
}
