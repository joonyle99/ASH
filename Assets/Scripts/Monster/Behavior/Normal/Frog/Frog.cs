using System.Threading;
using UnityEngine;

public class Frog : NormalMonster
{
    [Header("Frog")]
    [Space]

    [SerializeField] private float _jumpPowerX = 10f;
    [SerializeField] private float _jumpPowerY = 15f;

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

        if (IsDead)
            return;

        // Change to Attack State
        if (AttackEvaluator.IsTargetWithinAttackRange())
        {
            if (CurrentStateIs<FloatingPatrolState>() || CurrentStateIs<FloatingChaseState>())
                Animator.SetTrigger("Attack");
        }
    }

    protected override void SetUp()
    {
        base.SetUp();
    }

    public override void KnockBack(Vector2 forceVector)
    {
        base.KnockBack(forceVector);
    }

    public override void OnHitted(AttackInfo attackInfo)
    {
        base.OnHitted(attackInfo);
    }

    public override void Die()
    {
        base.Die();
    }

    public void Jump()
    {
        Vector2 forceVector = new Vector2(_jumpPowerX * RecentDir, _jumpPowerY);
        RigidBody.AddForce(forceVector, ForceMode2D.Impulse);
    }
}
