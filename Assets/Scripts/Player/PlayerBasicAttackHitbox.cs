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
        // ���� ������
        OncologySlime slime = collision.GetComponent<OncologySlime>();

        if (slime != null)
        {
            // �ݴ� ����
            float dir = Mathf.Sign(collision.transform.position.x - this.gameObject.transform.parent.position.x);
            Vector2 vec = new Vector2(2 * dir, 2 / 2f);

            slime.KnockBack(vec);   // �˹�
            slime.OnDamage(20);     // ������
        }
    }
}