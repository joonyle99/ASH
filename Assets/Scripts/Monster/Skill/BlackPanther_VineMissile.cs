using UnityEngine;

public class BlackPanther_VineMissile : Monster_ProjectileSkill
{
    public void Shoot(Vector3 dir, float speed)
    {
        var force = dir * speed;
        rigidBody2D.AddForce(force, ForceMode2D.Impulse);
    }
}
