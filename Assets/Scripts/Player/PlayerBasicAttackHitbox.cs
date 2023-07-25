using UnityEngine;

public class PlayerBasicAttackHitbox : MonoBehaviour
{
    public PlayerBehaviour Player { get; private set; }

    private void Awake()
    {
        Player = GetComponentInParent<PlayerBehaviour>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 종양 슬라임
        OncologySlime slime = collision.GetComponent<OncologySlime>();

        if (slime != null)
        {
            // 반대 방향
            float dir = Mathf.Sign(collision.transform.position.x - this.gameObject.transform.parent.position.x);
            Vector2 vec = new Vector2(2 * dir, 2 / 2f);

            slime.KnockBack(vec);   // 넉백
            slime.OnDamage(20);     // 데미지
        }
    }
}