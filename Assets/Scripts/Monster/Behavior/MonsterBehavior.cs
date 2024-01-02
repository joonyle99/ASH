using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// 몬스터의 기본 행동을 정의
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

    // 상태
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
    public MonsterDefine.SIZE MonsterSize // 몬스터 크기 구분
    {
        get => _monsterSize;
        protected set => _monsterSize = value;
    }
    [SerializeField] private MonsterDefine.MONSTER_TYPE _monsterType;
    public MonsterDefine.MONSTER_TYPE MonsterType // 몬스터 타입
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

        // State 세팅
        InitState();
    }

    protected virtual void Start()
    {
        // 몬스터 속성 설정
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
        // 몬스터의 ID 설정
        ID = _monsterData.ID;

        // 몬스터의 이름 설정
        MonsterName = _monsterData.MonsterName;

        // 몬스터의 타입 설정
        MonsterType = _monsterData.MonsterType;

        // 몬스터의 최대 체력
        MaxHp = _monsterData.MaxHp;
        CurHp = MaxHp;

        // 몬스터의 이동속도 설정
        MoveSpeed = _monsterData.MoveSpeed;

        // 몬스터의 크기 타입
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
        // 사망 처리
        IsDead = true;

        // Hit Box 비활성화
        GameObject hitBox = GetComponentInChildren<MonsterBodyHit>().gameObject;

        if (hitBox != null)
            hitBox.SetActive(false);

        // 사라지기 시작
        StartDestroy();
    }


    private IEnumerator AlphaBlink()
    {
        // 자식 오브젝트의 모든 SpriteRenderer를 가져온다
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        // TODO : Count 기반이 아닌 Hurt인 시간 동안 Blink
        for (int n = 0; n < _countOfBlink; n++)
        {
            // 모든 Sprite를 돌면서 깜빡임 효과를 적용
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                // 현재 색상을 투명하게 설정
                Color transparentColor = renderer.color;
                transparentColor.a = 0.5f; // 약간 투명하게 설정
                renderer.color = transparentColor;
            }

            yield return new WaitForSeconds(_blinkDuration);

            // 모든 Sprite를 돌면서 원래 색상으로 복구
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                // 현재 색상을 원래대로 설정
                Color originalColor = renderer.color;
                originalColor.a = 1f; // 완전 불투명하게 설정
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
        // 자식 오브젝트의 모든 SpriteRenderer를 가져온다
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        // 초기 알파값 저장
        float[] startAlphaArray = new float[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            startAlphaArray[i] = spriteRenderers[i].color.a;

        // 모든 렌더 컴포넌트를 돌면서 Fade Out
        while (_elapsedFadeOutTime < _targetFadeOutTime)
        {
            _elapsedFadeOutTime += Time.deltaTime;
            float normalizedTime = _elapsedFadeOutTime / _targetFadeOutTime; // Normalize to 0 ~ 1

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
        // Entry State의 정보 가져오기
        int initialPathHash = Animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        StateMachineBehaviour[] initialStates = Animator.GetBehaviours(initialPathHash, 0);
        foreach (var initialState in initialStates)
        {
            if (initialState as Monster_StateBase)
                _initialState = initialState as Monster_StateBase;
        }

        // Animation State 정보 초기화
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
