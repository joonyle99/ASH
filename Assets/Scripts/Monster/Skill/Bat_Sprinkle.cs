using UnityEngine;

public class Bat_Sprinkle : Monster_SkillObject
{
    [Header("Bat_Sprinkle")]
    [Space]

    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
}
