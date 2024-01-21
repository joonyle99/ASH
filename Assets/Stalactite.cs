using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stalactite : MonoBehaviour
{
    private Rigidbody2D _rigidbody;

    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private LayerMask _destroyLayer;

    [SerializeField] private float _fallingStartTime;
    [SerializeField] private float _elapsedTime;

    [SerializeField] private float _attackPowerX = 7f;
    [SerializeField] private float _attackPowerY = 10f;
    [SerializeField] private int _attackDamage = 20;

    [SerializeField] private GameObject ImpactPrefab;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _fallingStartTime = Random.Range(0.2f, 1.5f);
    }

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

    public void StartMessage()
    {
        Debug.Log(this.gameObject.name + " start to fall");

        StartCoroutine(Falling());
    }

    private IEnumerator Falling()
    {
        // ³«ÇÏ Àü Èçµé°Å¸®´Â ¿¬Ãâ

        yield return new WaitForSeconds(_fallingStartTime);

        // ¿¬Ãâ Á¾·á ÈÄ ³«ÇÏ
        _rigidbody.constraints = RigidbodyConstraints2D.None;
    }
}
