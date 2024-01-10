using System.Collections;
using System.Threading;
using UnityEngine;

public class Frog : MonsterBehavior
{
    [Header("Frog")]
    [Space]

    [SerializeField] private FrogTongueAttack _tonguePrefab;
    [SerializeField] private Transform _mouthTrans;

    [Space]

    [SerializeField] private float _tongueLength = 15f;
    [SerializeField] private float _targetTongueAttackTime = 0.1f;

    private FrogTongueAttack _tongueInstance;
    private SpriteRenderer _tongueSpriteRenderer;
    private Coroutine _tongueAttackCoroutine;

    protected override void Awake()
    {
        base.Awake();
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
    public override void OnHit(AttackInfo attackInfo)
    {
        base.OnHit(attackInfo);

        if (AttackEvaluator)
        {
            if (!AttackEvaluator.IsAttackable)
                AttackEvaluator.IsAttackable = true;
        }

        if (GroundChaseEvaluator)
        {
            if (!GroundChaseEvaluator.IsChasable)
                GroundChaseEvaluator.IsChasable = true;
        }
    }
    public override void Die()
    {
        base.Die();
    }

    public void Jump()
    {
        Vector2 forceVector = new Vector2(JumpForce.x * RecentDir, JumpForce.y);
        Rigidbody.AddForce(forceVector, ForceMode2D.Impulse);
    }

    public void TongueAttack()
    {
        _tongueInstance = Instantiate(_tonguePrefab, _mouthTrans.position, Quaternion.identity, _mouthTrans);
        _tongueSpriteRenderer = _tongueInstance.GetComponent<SpriteRenderer>();
        _tongueAttackCoroutine = StartCoroutine(ExtendTongue());
    }

    private IEnumerator ExtendTongue()
    {
        Vector2 startSize = _tongueSpriteRenderer.size;
        Vector2 targetSize = new Vector2(_tongueLength, _tongueSpriteRenderer.size.y);

        float elapsedTime = 0f;

        while (elapsedTime < _targetTongueAttackTime)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / _targetTongueAttackTime);
            if (_tongueSpriteRenderer)
                _tongueSpriteRenderer.size = Vector2.Lerp(startSize, targetSize, t);

            yield return null;
        }
    }

    public void DestoryTongue()
    {
        if (_tongueAttackCoroutine != null)
            StopCoroutine(ExtendTongue());

        Destroy(_tongueInstance.gameObject);
    }
}
