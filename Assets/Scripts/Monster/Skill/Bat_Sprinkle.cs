using UnityEngine;

public class Bat_Sprinkle : Monster_ProjectileSkill
{
    [Header("Bat_Sprinkle")]
    [Space]

    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;

    protected override void Awake()
    {
        base.Awake();

        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }

    public void Shoot(Vector3 throwForce)
    {
        _rigidbody.AddForce(throwForce, ForceMode2D.Impulse);
    }
}
