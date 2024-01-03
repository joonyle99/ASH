using UnityEngine;

public class PlayerBasicAttackHitbox : MonoBehaviour
{
    /*
    [SerializeField] private PlayerAttackController _playerAttackController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // you can't attack until your next attack animation begins.
        if (isDisableAttackHitBox) return;

        // check
        MonsterBodyHit monsterBodyHit = collision.GetComponent<MonsterBodyHit>();
        if (monsterBodyHit == null) return;

        MonsterBehavior monsterBehavior = collision.GetComponentInParent<MonsterBehavior>();
        if (monsterBehavior == null) return;

        isDisableAttackHitBox = true;

        // set forceVector
        Transform playerTrans = transform.root;
        Transform monsterTrans = collision.transform;
        float dir = Mathf.Sign(monsterTrans.position.x - playerTrans.position.x);
        Vector2 forceVector = new Vector2(_attackPowerX * dir, _attackPowerY);

        // OnHit() message to monsterBehavior
        monsterBehavior.OnHit(_attackDamage, forceVector);
    }
    */
}