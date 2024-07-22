using UnityEngine;

public class BlackPanther_VineMissile : Monster_IndependentSkill
{
    public void Shoot(Vector3 dir, float speed)
    {
        var force = dir * speed;
        GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
    }
}
