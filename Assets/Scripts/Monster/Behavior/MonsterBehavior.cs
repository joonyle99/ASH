using System.Collections;
using UnityEngine;

/// <summary>
/// ������ �⺻ �ൿ�� ����
/// </summary>
public abstract class MonsterBehavior : StateMachineBase
{
    #region Attribute

    [Header("Monster Behavior")]
    [Space]

    [SerializeField] private MonsterData _monsterData;
    [SerializeField] private WayPointPatrol _wayPointPatrol;
    public WayPointPatrol WayPointPatrol
    {
        get { return _wayPointPatrol; }
    }
    [SerializeField] private AttackEvaluator _attackEvaluator;
    public AttackEvaluator AttackEvaluators
    {
        get { return _attackEvaluator; }
    }

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
        set => _curHp = value;
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
    public bool IsDead
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
    [SerializeField] private MonsterDefine.SIZE _monsterSize;
    public MonsterDefine.SIZE MonsterSize // ���� ũ�� ����
    {
        get => _monsterSize;
        protected set => _monsterSize = value;
    }

    [SerializeField] private MonsterDefine.TYPE _monsterType;
    public MonsterDefine.TYPE MonsterType // ���� ����
    {
        get => _monsterType;
        protected set => _monsterType = value;
    }

    [SerializeField] private MonsterDefine.ACTION _actionType;
    public MonsterDefine.ACTION ActionType // ���� Ȱ�� ����
    {
        get => _actionType;
        protected set => _actionType = value;
    }

    [SerializeField] private MonsterDefine.RESPONE _responseType;
    public MonsterDefine.RESPONE ResponseType // ���� ��� ����
    {
        get => _responseType;
        protected set => _responseType = value;
    }

    [SerializeField] private MonsterDefine.AGGRESSIVE _aggressiveType;
    public MonsterDefine.AGGRESSIVE AggressiveType // ���� ����
    {
        get => _aggressiveType;
        protected set => _aggressiveType = value;
    }

    [SerializeField] private MonsterDefine.CHASE _chaseType;
    public MonsterDefine.CHASE ChaseType // ���� ��� ����
    {
        get => _chaseType;
        protected set => _chaseType = value;
    }

    [SerializeField] private MonsterDefine.RUNAWAY _runawayType;
    public MonsterDefine.RUNAWAY RunawayType // ���� ����
    {
        get => _runawayType;
        protected set => _runawayType = value;
    }

    [Space]

    [SerializeField] private float _elapsedFadeOutTime;
    [SerializeField] private float _targetFadeOutTime = 3f;

    #endregion

    #region Function

    /// <summary>
    /// ������Ʈ�� �پ��ִ� ��ũ��Ʈ�� ����� �� �� �� ����ȴ�.
    /// Ȱ��ȭ �Ǿ����� �ʾƵ� ����ȴ�.
    /// </summary>
    protected virtual void Awake()
    {
        _wayPointPatrol = GetComponent<WayPointPatrol>();
        _attackEvaluator = GetComponent<AttackEvaluator>();
    }

    protected override void Start()
    {
        base.Start();

        // ���� �Ӽ� ����
        SetUp();

        // ������ ���� ü��
        CurHp = MaxHp;
    }

    protected override void Update()
    {
        base.Update();

        if (CurHp <= 0 && !IsDead)
        {
            CurHp = 0;
            IsDead = true;
            Die();
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    // ���� �Ӽ� ����
    public virtual void SetUp()
    {
        // ������ ID ����
        ID = _monsterData.ID;

        // ������ �̸� ����
        MonsterName = _monsterData.MonsterName;

        // ������ Ÿ�� ����
        MonsterType = _monsterData.MonsterType;

        // ������ �ִ� ü��
        MaxHp = _monsterData.MaxHp;

        // ������ �̵��ӵ� ����
        MoveSpeed = _monsterData.MoveSpeed;

        // ������ ũ�� Ÿ��
        MonsterSize = _monsterData.MonsterSize;

        // ������ Ȱ�� ���� Ÿ��
        ActionType = _monsterData.ActionType;

        // ������ ���� Ÿ��
        ResponseType = _monsterData.ResponseType;

        // ������ ���� Ÿ��
        AggressiveType = _monsterData.AggressiveType;

        // ������ ���� Ÿ��
        ChaseType = _monsterData.ChaseType;

        // ������ ���� Ÿ��
        RunawayType = _monsterData.RunawayType;
    }

    public virtual void KnockBack(Vector2 forceVector)
    {
        Rigidbody.velocity = Vector2.zero;
        Rigidbody.AddForce(forceVector, ForceMode2D.Impulse);
    }

    public virtual void OnHit(int damage, Vector2 forceVector)
    {
        if (IsDead)
            return;

        // Apply Damage
        CurHp -= damage;

        // Apply Knock Back
        KnockBack(forceVector);

        GetComponent<SoundList>().PlaySFX("SE_Hurt");

        // Change Die State
        if (CurHp <= 0)
        {
            CurHp = 0;
            Die();
        }
    }

    // ���
    public virtual void Die()   // ������ �ڽ��� Die()�� ȣ���Ѵ�.
                                // Polymorphism (������)
    {
        // ��� ó��
        IsDead = true;

        // ��Ʈ �ڽ� ��Ȱ��ȭ
        GameObject hitBox = GetComponentInChildren<MonsterBodyHit>().gameObject;
        hitBox.SetActive(false);

        // ������� ����
        StartCoroutine(FadeOutObject());
    }

    public IEnumerator FadeOutObject()
    {
        // �ڽ� ������Ʈ�� ��� SpriteRenderer�� �����´�
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        // �ʱ� ���İ� ����
        float[] startAlphaArray = new float[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            startAlphaArray[i] = spriteRenderers[i].color.a;

        // ��� ���� ������Ʈ�� ���鼭 Fade Out
        while (_elapsedFadeOutTime < _targetFadeOutTime)
        {
            _elapsedFadeOutTime += Time.deltaTime;
            float normalizedTime = _elapsedFadeOutTime / 2;

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                // ���� ��������Ʈ �������� ���İ��� ����
                Color targetColor = spriteRenderers[i].color;
                targetColor.a = Mathf.Lerp(startAlphaArray[i], 0f, normalizedTime);
                spriteRenderers[i].color = targetColor;
            }

            yield return null;
        }

        // ������Ʈ ����
        if(transform.parent)
            Destroy(transform.parent.gameObject);
        else
            Destroy(gameObject);

        yield return null;
    }

    #endregion
}
