using System.Collections;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Bat : MonsterBehavior
{
    [Header("Bat")]
    [Space]

    [SerializeField] private Bat_Sprinkle _batSkillPrefab;
    [SerializeField] private Transform _shootPosition;

    [Space]

    [SerializeField] private Sprite[] _skillSprites;

    [Space]

    [SerializeField] private int _particleCount = 8;
    [SerializeField] private float _shootingPowerMin = 3f;
    [SerializeField] private float _shootingPowerMax = 10f;
    [SerializeField] private float _shootingAngle = 90f;
    [SerializeField] private float _shootingVariant = 40f;

    [Space]

    [SerializeField] private float _ShootingDebugLength = 10f;

    protected override void Start()
    {
        base.Start();

        if (NavMeshMovementModule)
        {
            NavMeshMovementModule.SetSpeed(MoveSpeed);
            NavMeshMovementModule.SetAcceleration(Acceleration);
        }
    }

    public void SprinkleParticle_AnimEvent()
    {
        for (int i = 0; i < _particleCount; i++)
        {
            // create particle
            Bat_Sprinkle particle = Instantiate(_batSkillPrefab, _shootPosition.position, Quaternion.identity);
            particle.SetSprite(_skillSprites[i % (_skillSprites.Length)]);

            // settings
            float angle = (i % 2 == 0) ? (180f - _shootingAngle) : _shootingAngle;
            float finalAngle = angle + Random.Range(-_shootingVariant, _shootingVariant);
            Vector3 dir = new Vector3(Mathf.Cos(finalAngle * Mathf.Deg2Rad), Mathf.Sin(finalAngle * Mathf.Deg2Rad), 0f).normalized;
            var power = Random.Range(_shootingPowerMin, _shootingPowerMax);

            Vector3 force = dir * power;

            // shoot particle
            particle.Shoot(force);
        }

        GetComponent<SoundList>().PlaySFX("SE_Bat");
    }

    void OnDrawGizmosSelected()
    {
        Vector3 vector1 = new Vector3(Mathf.Cos(_shootingAngle * Mathf.Deg2Rad), Mathf.Sin(_shootingAngle * Mathf.Deg2Rad), 0f).normalized;
        Vector3 vector2 = new Vector3(Mathf.Cos((180f - _shootingAngle) * Mathf.Deg2Rad), Mathf.Sin((180f - _shootingAngle) * Mathf.Deg2Rad), 0f).normalized;
        Vector3 vector1_1 = new Vector3(Mathf.Cos((_shootingAngle + _shootingVariant) * Mathf.Deg2Rad), Mathf.Sin((_shootingAngle + _shootingVariant) * Mathf.Deg2Rad), 0f).normalized;
        Vector3 vector1_2 = new Vector3(Mathf.Cos((_shootingAngle - _shootingVariant) * Mathf.Deg2Rad), Mathf.Sin((_shootingAngle - _shootingVariant) * Mathf.Deg2Rad), 0f).normalized;
        Vector3 vector2_1 = new Vector3(Mathf.Cos(((180f - _shootingAngle) + _shootingVariant) * Mathf.Deg2Rad), Mathf.Sin(((180f - _shootingAngle) + _shootingVariant) * Mathf.Deg2Rad), 0f).normalized;
        Vector3 vector2_2 = new Vector3(Mathf.Cos(((180f - _shootingAngle) - _shootingVariant) * Mathf.Deg2Rad), Mathf.Sin(((180f - _shootingAngle) - _shootingVariant) * Mathf.Deg2Rad), 0f).normalized;

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(_shootPosition.position, _shootPosition.position + vector1 * _ShootingDebugLength);
        Gizmos.DrawLine(_shootPosition.position, _shootPosition.position + vector2 * _ShootingDebugLength);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_shootPosition.position, _shootPosition.position + vector1_1 * _ShootingDebugLength);
        Gizmos.DrawLine(_shootPosition.position, _shootPosition.position + vector1_2 * _ShootingDebugLength);
        Gizmos.DrawLine(_shootPosition.position, _shootPosition.position + vector2_1 * _ShootingDebugLength);
        Gizmos.DrawLine(_shootPosition.position, _shootPosition.position + vector2_2 * _ShootingDebugLength);
    }
}
