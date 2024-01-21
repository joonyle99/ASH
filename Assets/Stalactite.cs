using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stalactite : MonoBehaviour
{
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private LayerMask _destroyLayer;

    [SerializeField] private float _attackPowerX = 7f;
    [SerializeField] private float _attackPowerY = 10f;
    [SerializeField] private int _attackDamage = 20;

    [SerializeField] private GameObject ImpactPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ÇÃ·¹ÀÌ¾î¿¡ ´êÀ¸¸é Å¸°Ý
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

        // ¶¥¿¡ ´êÀ¸¸é ÆÄ±«
        if ((1 << collision.gameObject.layer & _destroyLayer.value) > 0)
        {
            Destroy(gameObject);
        }
    }
}
