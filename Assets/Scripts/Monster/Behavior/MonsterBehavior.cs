using System.Collections;
using UnityEngine;

/// <summary>
/// ������ �⺻ �ൿ�� ����
/// </summary>
public abstract class MonsterBehavior : StateMachineBase
{
    #region Attribute

    [Header("Monster Behavior")] [Space] [SerializeField]
    private MonsterData _monsterData;

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

    private bool _isDieTrigger;

    #endregion

    #region Function

    /// <summary>
    /// ������Ʈ�� �پ��ִ� ��ũ��Ʈ�� ����� �� �� �� ����ȴ�.
    /// Ȱ��ȭ �Ǿ����� �ʾƵ� ����ȴ�.
    /// </summary>
    protected virtual void Awake()
    {
        // Component
        _collider2D = this.GetComponent<Collider2D>();

        if (!_collider2D)
            Debug.Log("Monster has not Collider2D Component");

        // ���� �Ӽ� ����
        SetUp();
    }

    protected override void Start()
    {
        base.Start();

        // ������ ���� ü��
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

    // ������ �ǰ�
    public virtual void OnDamage(int damage)
    {
        Debug.Log("MonsterBehavior�� OnDamage()");
        CurHp -= damage;

        if (CurHp <= 0)
            CurHp = 0;
    }

    // �˹�
    public virtual void KnockBack(Vector2 force)
    {
        Rigidbody.AddForce(force, ForceMode2D.Impulse);
    }

    // ���
    public virtual void Die()   // ������ �ڽ��� Die()�� ȣ���Ѵ�.
                                // Polymorphism (������)
    {
        Debug.Log("MonsterBehavior�� Die()");

        // �浹 ��Ȱ��ȭ
        if (_collider2D)
            _collider2D.enabled = false;

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
        Destroy(gameObject);

        yield return null;
    }

    #endregion
}
