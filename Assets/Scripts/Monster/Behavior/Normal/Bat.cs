using System.Threading;
using UnityEngine;

public class Bat : MonsterBehavior
{
    [Header("Bat")]
    [Space]

    [SerializeField] private BatSkillParticle _batSkillPrefab;
    [SerializeField] private Transform _shootPosition;
    [SerializeField] private Sprite[] _skillSprites;
    private int _particleCount = 8;
    private float _shootingPower = 6f;
    private float _shootingAngle = 60f;
    private float _shootingVariant = 30f;

    // Etc
    protected int countOfUpdate = 0;

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

        // TODO : temp code
        countOfUpdate++;

        if (countOfUpdate == 1)
            return;

        if (FloatingChaseEvaluator)
        {
            if (FloatingChaseEvaluator.IsTargetWithinRange())
            {
                if (CurrentStateIs<FloatingPatrolState>())
                {
                    //Debug.Log("SetTrigger Chase");
                    Animator.SetTrigger("Chase");
                    return;
                }
            }
            else
            {
                if (CurrentStateIs<FloatingChaseState>())
                {
                    //Debug.Log("SetTrigger Patrol");
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
    public override void OnHit(AttackInfo attackInfo)
    {
        base.OnHit(attackInfo);
    }
    public override void Die()
    {
        base.Die();
    }

    public void SprinkleParticle_AnimEvent()
    {
        for (int i = 0; i < _particleCount; i++)
        {
            BatSkillParticle particle = Instantiate(_batSkillPrefab, _shootPosition.position, Quaternion.identity);
            particle.SetSprite(_skillSprites[i % (_skillSprites.Length)]);
            float angle = (i % 2 == 0) ? _shootingAngle : -_shootingAngle;
            particle.Shoot(Random.Range(-_shootingVariant, _shootingVariant) + angle, _shootingPower);
        }

        GetComponent<SoundList>().PlaySFX("SE_Bat");
    }
}
