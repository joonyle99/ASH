using UnityEngine;

public class FireBullet : MonoBehaviour
{
    [Header("Bullet Setting")]

    [Space]

    [Range(0f, 30f)] [SerializeField] float _speed = 2.5f;
    [Range(0f, 20f)] [SerializeField] float _destroyTime = 3.0f;
    [Range(0f, 50f)] [SerializeField] float _knockPowerX = 10f;
    [Range(0f, 50f)] [SerializeField] float _knockPowerY = 5f;
    [Range(0, 100)] [SerializeField] int _damage = 30;

    float _time;

    void Update()
    {
        // x scale�� ��ȣ�� ���� ������ ����
        transform.position += transform.right * transform.localScale.x * _speed * Time.deltaTime;

        _time += Time.deltaTime;

        if(_time >= _destroyTime)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // ���Ϳ� �浹�� ���
        if (collision.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            float dir = Mathf.Sign(collision.transform.position.x - transform.position.x);
            Vector2 vec = new Vector2(_knockPowerX * dir, _knockPowerY);

            // �������� �˹�
            collision.GetComponent<BasedMonster>().OnDamage(_damage);
            collision.GetComponent<BasedMonster>().KnockBack(vec);

            Destroy(gameObject);
        }
    }
}
