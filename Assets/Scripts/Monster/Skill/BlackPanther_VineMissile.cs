using UnityEngine;

public class BlackPanther_VineMissile : Monster_ProjectileSkill
{
    [SerializeField] private GameObject projectileEffect;

    public void Shake()
    {
        projectileEffect.SetActive(true);
    }

    public void Shoot(Vector3 targetPos, float speed)
    {
        var moveDir = (targetPos - transform.position).normalized;
        var force = moveDir * speed;
        rigidBody2D.AddForce(force, ForceMode2D.Impulse);
    }
}
