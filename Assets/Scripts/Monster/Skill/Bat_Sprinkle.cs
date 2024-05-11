using UnityEngine;

public class Bat_Sprinkle : Monster_IndependentSkill
{
    [Header("Bat_Sprinkle")]
    [Space]

    private Rigidbody2D _rigid;
    private SpriteRenderer _spriteRenderer;

    protected override void Awake()
    {
        base.Awake();

        _rigid = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }

    public void Shoot(Vector3 throwForce)
    {
        _rigid.AddForce(throwForce, ForceMode2D.Impulse);
    }
}
