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

    [SerializeField] private int _particleCount = 8;
    [SerializeField] private float _shootingPower = 6f;
    [SerializeField] private float _shootingAngle = 90f;
    [SerializeField] private float _shootingVariant = 40f;

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
    public override IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        return base.OnHit(attackInfo);
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
