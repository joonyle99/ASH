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

    private bool _isEnd;

    protected override void OnEnter()
    {
        Animator.SetTrigger("Attack");

        _isEnd = false;

        StartCoroutine(Monster.AttackEvaluators.AttackableTimer());
    }

    protected override void OnUpdate()
    {
        if (_isEnd)
        {
            _isEnd = false;
            ChangeState<M_IdleState>();
            return;
        }
    }

    protected override void OnFixedUpdate()
    {

    }

    protected override void OnExit()
    {
        _isEnd = false;
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

    public void EndAttack_AnimEvent()
    {
        _isEnd = true;
    }
}
