using UnityEngine;

public class GroundMovementModule : MonoBehaviour
{
    private MonsterBehaviour _monster;

    void Awake()
    {
        _monster = GetComponent<MonsterBehaviour>();
    }

    public void AffectGravity(float gravityPower = 250f)
    {
        // ���߿� �ִ� ��� �߰����� �߷��� �������� �ʴ´�
        if (_monster.IsInAir)
            return;

        Vector2 groundNormal = _monster.groundRayHit.normal;
        float groundAngle = Mathf.Abs(Mathf.Atan2(groundNormal.y, groundNormal.x) * Mathf.Rad2Deg - 90f);

        // ������ ������ �̲��� ����
        if (groundAngle < 70f)
            _monster.RigidBody2D.AddForce(-groundNormal * gravityPower);
    }

    public void WalkGround()
    {
        // ���߿� �ִ� ��� �̵����� �ʴ´�
        if (_monster.IsInAir)
            return;

        // �߰��� ������ �Ÿ��� �ʹ� ��������� �߰��� �ߴ�
        if (_monster.GroundChaseEvaluator)
        {
            if (_monster.GroundChaseEvaluator.IsUsable)
            {
                if (_monster.GroundChaseEvaluator.IsTooClose)
                {
                    return;
                }
            }
        }

        Vector2 groundNormal = _monster.groundRayHit.normal;
        Vector2 moveDirection = _monster.RecentDir > 0
            ? (-1) * Vector2.Perpendicular(groundNormal)
            : Vector2.Perpendicular(groundNormal);

        Debug.DrawRay(_monster.groundRayHit.point, groundNormal, Color.yellow);

        Vector2 targetVelocity = moveDirection * _monster.monsterData.MoveSpeed;
        Vector2 projectedVelocity = Vector2.Dot(_monster.RigidBody2D.velocity, moveDirection) * moveDirection;
        Vector2 velocityNeeded = targetVelocity - projectedVelocity;
        Vector2 moveForce = velocityNeeded * _monster.monsterData.Acceleration;

        Debug.DrawRay(transform.position, moveForce, Color.red);

        _monster.RigidBody2D.AddForce(moveForce);
    }

    public void GroundJumpping()
    {
        if (_monster.IsInAir)
            return;

        Vector2 forceVector = new Vector2(_monster.monsterData.JumpForce.x * _monster.RecentDir, _monster.monsterData.JumpForce.y);
        _monster.RigidBody2D.AddForce(forceVector, ForceMode2D.Impulse);
    }
}
