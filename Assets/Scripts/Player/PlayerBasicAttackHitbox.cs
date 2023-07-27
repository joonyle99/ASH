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
    /// ���� Ÿ��
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Slime -> Normal -> Based �̷������� Ÿ�� ��
        // base.func() ���ϸ� Slime�� ����ȴ�
        BasedMonster monster = collision.GetComponent<BasedMonster>();

        if (monster != null)
        {
            // �ݴ� ����
            float dir = Mathf.Sign(collision.transform.position.x - this.gameObject.transform.parent.position.x);
            Vector2 vec = new Vector2(power * dir, power / 2f);

            Player.PlaySound_SE_Hurt_02();

            monster.KnockBack(vec);         // �˹�
            monster.OnDamage(damage);       // ������
        }
    }
}