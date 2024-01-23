using System.Collections;
using System.Threading;
using UnityEngine;

public class Bat : MonsterBehavior
{
    [Header("Bat")]
    [Space]

    [SerializeField] private Bat_Sprinkle _batSkillPrefab;
    [SerializeField] private Transform _shootPosition;
    [SerializeField] private Sprite[] _skillSprites;

    private int _particleCount = 8;
    private float _shootingPower = 6f;
    private float _shootingAngle = 60f;
    private float _shootingVariant = 30f;

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

        if (FloatingChaseEvaluator)
        {
            if (FloatingChaseEvaluator.IsTargetWithinRange())
            {
                if (CurrentStateIs<FloatingPatrolState>())
                {
                    Animator.SetTrigger("Chase");
                    return;
                }
            }
            else
            {
                if (CurrentStateIs<FloatingChaseState>())
                {
                    Animator.SetTrigger("Patrol");
                    return;
                }
            }
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
    public override IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        return base.OnHit(attackInfo);
    }
    protected override IEnumerator DeathEffectCoroutine()
    {
        var effect = GetComponent<DisintegrateEffect>();
        yield return new WaitForSeconds(0.3f);

        // Stop movement - Zero Velocity
        var navMeshMoveModule = GetComponent<NavMeshMoveModule>();
        navMeshMoveModule.SetVelocityZero();

        effect.Play();
        yield return new WaitUntil(() => effect.IsEffectDone);
    }

    public void SprinkleParticle_AnimEvent()
    {
        for (int i = 0; i < _particleCount; i++)
        {
            Bat_Sprinkle particle = Instantiate(_batSkillPrefab, _shootPosition.position, Quaternion.identity);
            particle.SetSprite(_skillSprites[i % (_skillSprites.Length)]);
            float angle = (i % 2 == 0) ? _shootingAngle : -_shootingAngle;
            particle.Shoot(Random.Range(-_shootingVariant, _shootingVariant) + angle, _shootingPower);
        }

        GetComponent<SoundList>().PlaySFX("SE_Bat");
    }
}
