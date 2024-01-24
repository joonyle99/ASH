using System.Collections;
using System.Threading;
using UnityEngine;

public class Frog : MonsterBehavior
{
    [Header("Frog")]
    [Space]

    [SerializeField] private Frog_Tongue _tonguePrefab;
    [SerializeField] private Transform _mouthTrans;

    [Space]

    [SerializeField] private float _tongueLength = 15f;
    [SerializeField] private float _targetTongueAttackTime = 0.1f;

    private Frog_Tongue _tongueInstance;
    private Coroutine _tongueAttackCoroutine;

    protected override void Awake()
    {
        base.Awake();

        AnimTransitionEvent -= PatrolToOtherCondition;
        AnimTransitionEvent += PatrolToOtherCondition;
    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
    }
    protected override void SetUp()
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

    public void TongueAttack_AnimEvent()
    {
        _tongueInstance = Instantiate(_tonguePrefab, _mouthTrans.position, Quaternion.identity, _mouthTrans);
        SpriteRenderer tongueSpriteRenderer = _tongueInstance.GetComponent<SpriteRenderer>();
        _tongueAttackCoroutine = StartCoroutine(ExtendTongue(tongueSpriteRenderer));
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

        Destroy(_tongueInstance.gameObject);
    }

    private bool PatrolToOtherCondition(string targetTransitionParam, Monster_StateBase currentState)
    {
        if (currentState is GroundMoveState)
        {
            if (targetTransitionParam == "Idle" || targetTransitionParam == "Attack")
            {
                if (IsGround) return true;
                else return false;
            }
        }

        return true;
    }
}
