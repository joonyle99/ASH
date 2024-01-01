using UnityEngine;

/// <summary>
/// 박쥐 몬스터 클래스
/// </summary>
public class Bat : NormalMonster
{
    [Header("Bat")]
    [Space]

    [SerializeField] private BatSkillParticle _batSkillPrefab;
    [SerializeField] private Transform _shootPosition;
    [SerializeField] private Sprite[] _skillSprites;
    [SerializeField] private int _particleCount;
    [SerializeField] private float _shootingPower;
    [SerializeField] private float _shootingAngle;
    [SerializeField] private float _shootingVariant;

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

    public override void OnHit(int damage, Vector2 forceVector)
    {
        base.OnHit(damage, forceVector);
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
