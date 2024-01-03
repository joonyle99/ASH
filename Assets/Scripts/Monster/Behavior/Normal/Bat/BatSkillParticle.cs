using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatSkillParticle : MonoBehaviour
{
    [SerializeField] LayerMask _groundLayer;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;

    [SerializeField] private int _damage;
    [SerializeField] private float _forceXPower;
    [SerializeField] private float _forceYPower;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void SetSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }

    public void Shoot(float angle, float power)
    {
        float afterAngle = Mathf.Deg2Rad * (angle + 90);
        Vector3 throwForce = new Vector3(Mathf.Cos(afterAngle), Mathf.Sin(afterAngle), 0) * power;

        _rigidbody.AddForce(throwForce, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & _groundLayer.value) > 0)
        {
            Destroy(gameObject);
            return;
        }

        PlayerBehaviour player = collision.transform.GetComponent<PlayerBehaviour>();

        if (player != null)
        {
            if (player.IsGodMode || player.IsDead)
                return;

            float dir = Mathf.Sign(player.transform.position.x - transform.position.x);
            Vector2 forceVector = new Vector2(_forceXPower * dir, _forceYPower);

            player.OnHit(_damage, forceVector);

            Destroy(gameObject);
        }
    }
}
