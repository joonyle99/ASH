using UnityEditor.Rendering;
using UnityEngine;

public class PlayerBasicAttackHitbox : MonoBehaviour
{
    public PlayerBehaviour Player { get; private set; }
    public int damage = 20;
    public float power;

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
        // Slime -> Normal -> Based 이런식으로 타고 들어감
        // base.func() 안하면 Slime만 실행된다
        BasedMonster monster = collision.GetComponent<BasedMonster>();

        if (monster != null)
        {
            // 반대 방향
            float dir = Mathf.Sign(collision.transform.position.x - this.gameObject.transform.parent.position.x);
            Vector2 vec = new Vector2(power * dir, power / 2f);

            Player.PlaySound_SE_Hurt_02();

            monster.KnockBack(vec);         // 넉백
            monster.OnDamage(damage);       // 데미지
        }
    }
}