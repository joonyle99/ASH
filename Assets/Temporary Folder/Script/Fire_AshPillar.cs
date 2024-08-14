using UnityEngine;

public class Fire_AshPillar : Monster_IndependentSkill
{
    // [Header("____ AshPillar ____")]
    // [Space]

    private float _speed = 10f;
    private Vector3 _direction = Vector3.zero;

    public void Update()
    {
        transform.position += _direction * _speed * Time.deltaTime;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }
    public void SetDirection(Vector3 dir)
    {
        _direction = dir;
    }
}
