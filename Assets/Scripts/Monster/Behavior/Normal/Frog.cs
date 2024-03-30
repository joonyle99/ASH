using UnityEngine;

public class Frog : MonsterBehavior
{
    /*
    [Header("Frog")]
    [Space]

    [SerializeField] private Frog_Tongue _tonguePrefab;

    [Space]

    [SerializeField] private float _tongueLength = 15f;
    [SerializeField] private float _targetTongueAttackTime = 0.1f;

    private SpriteRenderer _tongueSpriteRenderer;
    private Vector2 _originSize;

    private Coroutine _tongueAttackCoroutine;
    */

    protected override void Awake()
    {
        base.Awake();

        // _tongueSpriteRenderer = _tonguePrefab.GetComponent<SpriteRenderer>();
        // _originSize = _tongueSpriteRenderer.size;

        customAnimTransitionEvent -= GroundMoveToOtherCondition;
        customAnimTransitionEvent += GroundMoveToOtherCondition;
    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
    }
    public override void SetUp()
    {
        base.SetUp();
    }

    public override void KnockBack(Vector2 forceVector)
    {
        base.KnockBack(forceVector);
    }
    public override IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        base.OnHit(attackInfo);

        // 선공 당하면 공격 모드 진입
        if (AttackEvaluator)
        {
            if (!AttackEvaluator.IsUsable)
                AttackEvaluator.IsUsable = true;
        }

        // 선공 당하면 추격 모드 진입
        if (GroundChaseEvaluator)
        {
            if (!GroundChaseEvaluator.IsUsable)
                GroundChaseEvaluator.IsUsable = true;
        }

        return IAttackListener.AttackResult.Success;
    }
    public override void Die()
    {
        base.Die();
    }

    public void Jump_AnimEvent()
    {
        Vector2 forceVector = new Vector2(JumpForce.x * RecentDir, JumpForce.y);
        Rigidbody.AddForce(forceVector, ForceMode2D.Impulse);
    }

    /*
    public void TongueAttack_AnimEvent()
    {
        _tonguePrefab.gameObject.SetActive(true);
        _tongueAttackCoroutine = StartCoroutine(ExtendTongue(_tongueSpriteRenderer));
    }
    private IEnumerator ExtendTongue(SpriteRenderer tongueSpriteRenderer)
    {
        Vector2 startSize = tongueSpriteRenderer.size;
        Vector2 targetSize = new Vector2(_tongueLength, tongueSpriteRenderer.size.y);

        float elapsedTime = 0f;

        while (elapsedTime < _targetTongueAttackTime)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / _targetTongueAttackTime);
            if (tongueSpriteRenderer)
                tongueSpriteRenderer.size = Vector2.Lerp(startSize, targetSize, t);

            yield return null;
        }
    }
    public void DestoryTongue_AnimEvent()
    {
        if (_tongueAttackCoroutine != null)
            StopCoroutine(_tongueAttackCoroutine);

        _tongueSpriteRenderer.size = _originSize;

        _tonguePrefab.gameObject.SetActive(false);
    }
    */

    private bool GroundMoveToOtherCondition(string targetTransitionParam, Monster_StateBase currentState)
    {
        if (currentState is GroundMoveState)
        {
            if (targetTransitionParam == "Idle" || targetTransitionParam == "Attack")
            {
                // 땅에 착지한 후 전이
                return IsGround;
            }
        }

        return true;
    }
}
