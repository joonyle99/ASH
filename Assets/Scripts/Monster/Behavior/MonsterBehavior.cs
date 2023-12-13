using UnityEngine;

#region Enum

public enum MONSTER_SIZE
{
    Null = 0,
    Small,
    Medium,
    Large
}

public enum MONSTER_TYPE
{
    Null = 0,
    Normal,
    SemiBoss,
    Boss
}

public enum ACTION_TYPE
{
    Null = 0,
    Ground,
    Floating,
    Crawl
}

public enum RESPONE_TYPE
{
    Null = 0,
    Time,
    Reentry,
    None
}

public enum AGGRESSIVE_TYPE
{
    Null = 0,
    Peace,
    SightAggressive,
    TerritoryAggressive,
    AttackAggressive,
    Boss
}

public enum CHASE_TYPE
{
    Null = 0,
    Peace,
    Territory,
    AllTerritory
}

public enum RUNAWAY_TYPE
{
    Null = 0,
    Aggressive,
    Peace,
    Coward
}

#endregion

/// <summary>
/// ������ �⺻ �ൿ�� ����
/// </summary>
public abstract class MonsterBehavior : StateMachineBase
{
    #region Attribute

    [Header("Monster Behavior")]
    [Space]

    [SerializeField] private Collider2D _collider2D;
    public Collider2D MonsterCollider2D { get => _collider2D; }

    // ���� �ĺ� ��ȣ ID
    [SerializeField] private int _id;
    public int ID
    {
        get => _id;
        protected set => _id = value;
    }

    // ���� �̸�
    [SerializeField] private string _monsterName;
    public string MonsterName
    {
        get => _monsterName;
        protected set => _monsterName = value;
    }

    // �ִ� ü��
    [SerializeField] private int _maxHp;
    public int MaxHp
    {
        get => _maxHp;
        protected set => _maxHp = value;
    }

    // ���� ü��
    [SerializeField] private int _curHp;
    public int CurHp
    {
        get => _curHp;
        protected set => _curHp = value;
    }

    // �̵��ӵ�
    [SerializeField] private float _moveSpeed;
    public float MoveSpeed
    {
        get => _moveSpeed;
        protected set => _moveSpeed = value;
    }

    [Space]

    // �� �� �Ӽ���
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

    // �߰� ������Ƽ
    [SerializeField] private MONSTER_SIZE _monsterSize;
    public MONSTER_SIZE MonsterSize // ���� ũ�� ����
    {
        get => _monsterSize;
        protected set => _monsterSize = value;
    }

    [SerializeField] private MONSTER_TYPE _monsterType;
    public MONSTER_TYPE MonsterType // ���� ����
    {
        get => _monsterType;
        protected set => _monsterType = value;
    }

    [SerializeField] private ACTION_TYPE _actionType;
    public ACTION_TYPE ActionType // ���� Ȱ�� ����
    {
        get => _actionType;
        protected set => _actionType = value;
    }

    [SerializeField] private RESPONE_TYPE _responseType;
    public RESPONE_TYPE ResponseType // ���� ��� ����
    {
        get => _responseType;
        protected set => _responseType = value;
    }

    [SerializeField] private AGGRESSIVE_TYPE _aggressiveType;
    public AGGRESSIVE_TYPE AggressiveType // ���� ����
    {
        get => _aggressiveType;
        protected set => _aggressiveType = value;
    }

    [SerializeField] private CHASE_TYPE _chaseType;
    public CHASE_TYPE ChaseType // ���� ��� ����
    {
        get => _chaseType;
        protected set => _chaseType = value;
    }

    [SerializeField] private RUNAWAY_TYPE _runawayType;
    public RUNAWAY_TYPE RunawayType // ���� ����
    {
        get => _runawayType;
        protected set => _runawayType = value;
    }


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

    // ���� �Ӽ� ����
    public abstract void SetUp();

    // ������ �ǰ�
    public virtual void OnDamage(int damage)
    {
        Debug.Log("MonsterBehavior�� OnDamage()");
        CurHp -= damage;

        if (CurHp <= 0)
        {
            CurHp = 0;
            Die();  // ������ �ڽ��� Die()�� ȣ���Ѵ�.
                    // Polymorphism (������)
        }
    }

    // �˹�
    public virtual void KnockBack(Vector2 vec)
    {

    }

    // ���
    public virtual void Die()
    {
        Debug.Log("MonsterBehavior�� Die()");

        // �浹 ��Ȱ��ȭ
        if (_collider2D)
            _collider2D.enabled = false;

        Dead = true;
    }

    #endregion
}
