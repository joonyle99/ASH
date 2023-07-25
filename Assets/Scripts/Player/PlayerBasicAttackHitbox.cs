using UnityEngine;

public class PlayerBasicAttackHitbox : MonoBehaviour
{
    public PlayerBehaviour Player { get; private set; }
    public int damage = 20;
    public float power = 1000f;

    private void Awake()
    {
        Player = GetComponentInParent<PlayerBehaviour>();
    }

    /// <summary>
    /// 몬스터 타격
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        BasedMonster monster = collision.GetComponent<BasedMonster>();

        if (monster != null)
        {
            // 반대 방향
            float dir = Mathf.Sign(collision.transform.position.x - this.gameObject.transform.parent.position.x);
            Vector2 vec = new Vector2(power * dir, power / 2f);

            monster.KnockBack(vec);         // 넉백
            monster.OnDamage(damage);       // 데미지
        }
    }
}