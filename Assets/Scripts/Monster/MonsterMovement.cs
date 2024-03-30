using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    private MonsterBehavior _monster;

    void Awake()
    {
        _monster = GetComponent<MonsterBehavior>();
    }

    public void GroundWalking()
    {
        if (_monster.IsInAir)
            return;

        // 추가로 상대와의 거리가 너무 가까워지면 추격을 중단
        if (_monster.GroundChaseEvaluator)
        {
            if (_monster.GroundChaseEvaluator.IsTooClose)
                return;
        }

        Vector2 groundNormal = _monster.GroundRayHit.normal;
        Vector2 moveDirection = _monster.RecentDir > 0
            ? (-1) * Vector2.Perpendicular(groundNormal)
            : Vector2.Perpendicular(groundNormal);

        Debug.DrawRay(_monster.GroundRayHit.point, groundNormal, Color.cyan);

        Vector2 targetVelocity = moveDirection * _monster.MoveSpeed;
        Vector2 velocityNeeded = targetVelocity - Vector2.Dot(_monster.Rigidbody.velocity, moveDirection) * moveDirection;   // 경사면을 따라 움직이기 위한 벡터
        Vector2 moveForce = velocityNeeded * _monster.Acceleration;

        Debug.DrawRay(transform.position, moveDirection, Color.cyan);

        _monster.Rigidbody.AddForce(moveForce);
    }
}
