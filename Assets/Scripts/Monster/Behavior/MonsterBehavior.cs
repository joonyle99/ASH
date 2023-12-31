using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// 몬스터의 기본 행동을 정의
/// </summary>
public abstract class MonsterBehavior : MonoBehaviour
{
    #region Attribute

    [Header("Monster Behavior")]
    [Space]

    // State
    [SerializeField] private Monster_StateBase _initialState;
    [SerializeField] private Monster_StateBase _currentState;
    public Monster_StateBase CurrentState
    {
        get { return _currentState; }
        set { _currentState = value; }
    }

    [Space]

    [SerializeField] private Rigidbody2D _rigidBody;
    public Rigidbody2D RigidBody
    {
        get { return _rigidBody; }
    }

    [SerializeField] private Animator _animator;
    public Animator Animator
    {
        get { return _animator; }
    }

    [Space]

    [SerializeField] private MonsterData _monsterData;

    [SerializeField] private WayPointPatrol _wayPointPatrol;
    public WayPointPatrol WayPointPatrol
    {
        get { return _wayPointPatrol; }
    }

    [SerializeField] private BasicAttackEvaluator _basicAttackEvaluator;
    public BasicAttackEvaluator BasicAttackEvaluator
    {
        get { return _basicAttackEvaluator; }
    }

    [Space]

    // 그 외 속성들
    [SerializeField] private bool _isDead;
    public bool IsDead
    {
        get => _isDead;
        set => _isDead = value;
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

    [Header("Monster Data")]
    [Space]

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
        set => _curHp = value;
    }

    // 이동속도
    [SerializeField] private float _moveSpeed;
    public float MoveSpeed
    {
        get => _moveSpeed;
        protected set => _moveSpeed = value;
    }

    // 추가 프로퍼티
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

    #endregion

    #region Function

    protected virtual void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        _wayPointPatrol = GetComponent<WayPointPatrol>();
        _basicAttackEvaluator = GetComponent<BasicAttackEvaluator>();

        // State 세팅
        int initialPathHash = Animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        StateMachineBehaviour[] initialStates = Animator.GetBehaviours(initialPathHash, 0);

        foreach (var initialState in initialStates)
        {
            if (initialState as Monster_StateBase)
                _initialState = initialState as Monster_StateBase;
        }

        _currentState = _initialState;
    }

    protected virtual void Start()
    {
        // 몬스터 속성 설정
        SetUp();

        // 박쥐의 체력 초기화
        CurHp = MaxHp;
    }

    protected virtual void Update()
    {
        if (IsDead)
            return;

        CheckDieState();

        // 공격 범위 안에 타겟이 들어오면
        if (BasicAttackEvaluator.IsTargetWithinAttackRange())
        {
            var stateInfo = Animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("Idle") || stateInfo.IsName("Move"))
                Animator.SetTrigger("Attack");
        }
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
        StartCoroutine(FadeOutDestroy());
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

    private void CheckDieState()
    {
        if (CurHp <= 0)
        {
            CurHp = 0;
            Animator.SetTrigger("Die");
        }
    }

    #endregion
}
