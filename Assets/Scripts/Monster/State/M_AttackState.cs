using UnityEngine;

public class M_AttackState : MonsterState
{
    [SerializeField] private BatSkillParticle _batSkillPrefab;
    [SerializeField] private Transform _shootPosition;
    [SerializeField] private Sprite[] _skillSprites;
    [SerializeField] private int _particleCount;
    [SerializeField] private float _shootingPower;
    [SerializeField] private float _shootingAngle;
    [SerializeField] private float _shootingVariant;

    protected override void OnEnter()
    {
        Animator.SetTrigger("Attack");

        StartCoroutine(Monster.AttackEvaluators.AttackableTimer());
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnFixedUpdate()
    {

    }

    protected override void OnExit()
    {

    }

    public void SprinkleParticle()
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

    public void EndAttack_AnimEvent()
    {
        ChangeState<M_IdleState>();
    }
}
