using UnityEditor.Rendering;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerBasicAttackHitbox : MonoBehaviour
{
    [SerializeField] private PlayerBehaviour _player;

    [SerializeField] private int _attackDamage = 20;
    [SerializeField] private float _attackPowerX;
    [SerializeField] private float _attackPowerY;

    private void Awake()
    {
        _player = transform.root.GetComponent<PlayerBehaviour>();
    }

    /// <summary>
    /// 몬스터 타격
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        MonsterBodyHit monsterBodyHit = collision.GetComponent<MonsterBodyHit>();
        if (monsterBodyHit == null)
            return;

        MonsterBehavior monsterBehavior = collision.GetComponentInParent<MonsterBehavior>();
        if (monsterBehavior == null)
            return;

        float dir = Mathf.Sign(collision.transform.position.x - transform.root.position.x);
        Vector2 forceVector = new Vector2(_attackPowerX * dir, _attackPowerY);

        monsterBehavior.OnHit(_attackDamage, forceVector);
    }
}