using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatSkillParticle : MonoBehaviour
{
    [SerializeField] LayerMask _groundLayer;
    SpriteRenderer _spriteRenderer;
    Rigidbody2D _rigidbody;
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
        angle = Mathf.Deg2Rad * (angle + 90);
        var force = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * power;
        _rigidbody.AddForce(force);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((1 << collision.gameObject.layer & _groundLayer) > 0)
        {
            Destroy(gameObject);
            return;
        }
        var player = collision.transform.GetComponent<PlayerBehaviour>();
        if (player)
        {
            player.OnHitByBatSkill(this);
            Destroy(gameObject);
        }
    }
}
