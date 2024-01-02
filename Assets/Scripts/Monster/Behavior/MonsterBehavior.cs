using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// ������ �⺻ �ൿ�� ����
/// </summary>
public abstract class MonsterBehavior : MonoBehaviour
{
    #region Attribute

    [Header("State")]
    [Space]

    // State
    [SerializeField] private Monster_StateBase _initialState;
    [SerializeField] private Monster_StateBase _currentState;
    [SerializeField] private Monster_StateBase _previousState;

    [Header("Module")]
    [Space]
    [SerializeField] private WayPointPatrol _wayPointPatrol;
    public WayPointPatrol WayPointPatrol
    {
        get { return _wayPointPatrol; }
    }
    [SerializeField] private NavMeshMove _navMeshMove;
    public NavMeshMove NavMeshMove
    {
        get { return _navMeshMove; }
    }
    [SerializeField] private PatrolEvaluator _patrolEvaluator;
    public PatrolEvaluator PatrolEvaluator
    {
        get { return _patrolEvaluator; }
    }
    [SerializeField] private ChaseEvaluator _chaseEvaluator;
    public ChaseEvaluator ChaseEvaluator
    {
        get { return _chaseEvaluator; }
    }
    [SerializeField] private AttackEvaluator _attackEvaluator;
    public AttackEvaluator AttackEvaluator
    {
        get { return _attackEvaluator; }
    }

    [Header("Condition")]
    [Space]

    // ����
    [SerializeField] private bool _isDead;
    public bool IsDead
    {
        get => _isDead;
        set => _isDead = value;
    }
    [SerializeField] private bool _isGodMode;
    public bool IsGodMode
    {
        get => _isGodMode;
        set => _isGodMode = value;
    }

    [Header("Data")]
    [Space]

    [SerializeField] private MonsterData _monsterData;

    [SerializeField] private int _id;
    public int ID
    {
        get => _id;
        protected set => _id = value;
    }
    [SerializeField] private string _monsterName;
    public string MonsterName
    {
        get => _monsterName;
        protected set => _monsterName = value;
    }
    [SerializeField] private int _maxHp;
    public int MaxHp
    {
        get => _maxHp;
        protected set => _maxHp = value;
    }
    [SerializeField] private int _curHp;
    public int CurHp
    {
        get => _curHp;
        set => _curHp = value;
    }
    [SerializeField] private float _moveSpeed;
    public float MoveSpeed
    {
        get => _moveSpeed;
        protected set => _moveSpeed = value;
    }
    [SerializeField] private MonsterDefine.SIZE _monsterSize;
    public MonsterDefine.SIZE MonsterSize // ���� ũ�� ����
    {
        get => _monsterSize;
        protected set => _monsterSize = value;
    }
    [SerializeField] private MonsterDefine.MONSTER_TYPE _monsterType;
    public MonsterDefine.MONSTER_TYPE MonsterType // ���� Ÿ��
    {
        get => _monsterType;
        protected set => _monsterType = value;
    }

    [Header("FadeOut")]
    [Space]

    [SerializeField] private float _targetFadeOutTime = 3f;
    [SerializeField] private float _elapsedFadeOutTime = 0f;

    [Header("Blink")]
    [Space]

    [SerializeField] private int _countOfBlink = 5;
    [SerializeField] private float _blinkDuration = 0.1f;

    public Rigidbody2D RigidBody { get; private set; }
    public Animator Animator { get; private set; }

    #endregion

    #region Function

    protected virtual void Awake()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();

        _wayPointPatrol = GetComponent<WayPointPatrol>();
        _navMeshMove = GetComponent<NavMeshMove>();
        _patrolEvaluator = GetComponent<PatrolEvaluator>();
        _chaseEvaluator = GetComponent<ChaseEvaluator>();
        _attackEvaluator = GetComponent<AttackEvaluator>();

        // State ����
        InitState();
    }

    protected virtual void Start()
    {
        // ���� �Ӽ� ����
        SetUp();
    }

    protected virtual void Update()
    {
        if (IsDead)
            return;

        CheckDie();
    }

    protected virtual void FixedUpdate()
    {

    }

    protected virtual void SetUp()
    {
        // ������ ID ����
        ID = _monsterData.ID;

        // ������ �̸� ����
        MonsterName = _monsterData.MonsterName;

        // ������ Ÿ�� ����
        MonsterType = _monsterData.MonsterType;

        // ������ �ִ� ü��
        MaxHp = _monsterData.MaxHp;
        CurHp = MaxHp;

        // ������ �̵��ӵ� ����
        MoveSpeed = _monsterData.MoveSpeed;

        // ������ ũ�� Ÿ��
        MonsterSize = _monsterData.MonsterSize;
    }

    public virtual void KnockBack(Vector2 forceVector)
    {
        RigidBody.velocity = Vector2.zero;
        RigidBody.AddForce(forceVector, ForceMode2D.Impulse);
    }

    public virtual void OnHit(int damage, Vector2 forceVector)
    {
        if (IsDead)
            return;

        if (IsGodMode)
            return;

        Debug.Log("Monster OnHit");

        CurHp -= damage;
        KnockBack(forceVector);
        GetComponent<SoundList>().PlaySFX("SE_Hurt");

        if (CurHp <= 0)
        {
            CurHp = 0;
            Animator.SetTrigger("Die");

            return;
        }

        Animator.SetTrigger("Hurt");
    }

    public virtual void Die()
    {
        // ��� ó��
        IsDead = true;

        // Hit Box ��Ȱ��ȭ
        GameObject hitBox = GetComponentInChildren<MonsterBodyHit>().gameObject;

        if (hitBox != null)
            hitBox.SetActive(false);

        // ������� ����
        StartDestroy();
    }


    private IEnumerator AlphaBlink()
    {
        // �ڽ� ������Ʈ�� ��� SpriteRenderer�� �����´�
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        // TODO : Count ����� �ƴ� Hurt�� �ð� ���� Blink
        for (int n = 0; n < _countOfBlink; n++)
        {
            // ��� Sprite�� ���鼭 ������ ȿ���� ����
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                // ���� ������ �����ϰ� ����
                Color transparentColor = renderer.color;
                transparentColor.a = 0.5f; // �ణ �����ϰ� ����
                renderer.color = transparentColor;
            }

            yield return new WaitForSeconds(_blinkDuration);

            // ��� Sprite�� ���鼭 ���� �������� ����
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                // ���� ������ ������� ����
                Color originalColor = renderer.color;
                originalColor.a = 1f; // ���� �������ϰ� ����
                renderer.color = originalColor;
            }

            yield return new WaitForSeconds(_blinkDuration);
        }
    }

    public void StartBlink()
    {
        StartCoroutine(AlphaBlink());
    }


    private IEnumerator FadeOutDestroy()
    {
        // �ڽ� ������Ʈ�� ��� SpriteRenderer�� �����´�
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        // �ʱ� ���İ� ����
        float[] startAlphaArray = new float[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            startAlphaArray[i] = spriteRenderers[i].color.a;

        // ��� ���� ������Ʈ�� ���鼭 Fade Out
        while (_elapsedFadeOutTime < _targetFadeOutTime)
        {
            _elapsedFadeOutTime += Time.deltaTime;
            float normalizedTime = _elapsedFadeOutTime / _targetFadeOutTime; // Normalize to 0 ~ 1

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
        if (transform.root)
            Destroy(transform.root.gameObject);
        else
            Destroy(gameObject);

        yield return null;
    }

    public void StartDestroy()
    {
        StartCoroutine(FadeOutDestroy());
    }

    private void CheckDie()
    {
        if (CurHp <= 0)
        {
            CurHp = 0;
            Animator.SetTrigger("Die");
        }
    }


    private void InitState()
    {
        // Entry State�� ���� ��������
        int initialPathHash = Animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        StateMachineBehaviour[] initialStates = Animator.GetBehaviours(initialPathHash, 0);
        foreach (var initialState in initialStates)
        {
            if (initialState as Monster_StateBase)
                _initialState = initialState as Monster_StateBase;
        }

        // Animation State ���� �ʱ�ȭ
        _currentState = _initialState;
        _previousState = _initialState;
    }

    public void UpdateState(Monster_StateBase state)
    {
        _previousState = _currentState;
        _currentState = state;
    }

    public bool CurrentStateIs<State>() where State : Monster_StateBase
    {
        return _currentState is State;
    }

    public bool PreviousStateIs<State>() where State : Monster_StateBase
    {
        return _previousState is State;
    }

    #endregion
}
