using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSkill : MonoBehaviour
{
    [SerializeField] private LayerMask _targetLayer;

    [SerializeField] private float _attackPowerX = 7f;
    [SerializeField] private float _attackPowerY = 10f;
    [SerializeField] private int _attackDamage = 20;

    [SerializeField] private GameObject ImpactPrefab;

    [SerializeField] private Vector2 _dir = Vector2.zero;

    [SerializeField] private float _targetDistance = 14f;   // 14유닛
    [SerializeField] private float _elapsedDistance;

    private void Update()
    {
        var frameDistance = _dir * Time.deltaTime * 14f;

        transform.Translate(frameDistance);
        _elapsedDistance += frameDistance.magnitude;

        if (_elapsedDistance >= _targetDistance)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어에 닿으면 타격
        if ((1 << collision.gameObject.layer & _targetLayer.value) > 0)
        {
            PlayerBehaviour player = collision.GetComponent<PlayerBehaviour>();
            if (player)
            {
                IAttackListener.AttackResult attackResult = IAttackListener.AttackResult.Fail;

                Vector2 forceVector = new Vector2(_attackPowerX * Mathf.Sign(player.transform.position.x - transform.position.x), _attackPowerY);

                var result = player.OnHit(new AttackInfo(_attackDamage, forceVector, AttackType.GimmickAttack));
                if (result == IAttackListener.AttackResult.Success)
                    attackResult = IAttackListener.AttackResult.Success;

                if (attackResult == IAttackListener.AttackResult.Success)
                {
                    Vector2 playerPos = player.transform.position;
                    Instantiate(ImpactPrefab, playerPos + Random.insideUnitCircle * 0.3f, Quaternion.identity);
                }
            }
        }
    }

    public void SetDir(Vector2 dir)
    {
        _dir = dir;
    }
}
