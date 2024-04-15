using UnityEngine;

public class GroundMovementModule : MonoBehaviour
{
    private MonsterBehavior _monster;

    void Awake()
    {
        _monster = GetComponent<MonsterBehavior>();
    }

    public void GroundWalking()
    {
        // ���߿� �ִ� ��� �̵����� �ʴ´�
        if (_monster.IsInAir)
        {
            return;
        }

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

        Debug.DrawRay(_monster.groundRayHit.point, groundNormal, Color.cyan);

        Vector2 targetVelocity = moveDirection * _monster.monsterData.MoveSpeed;
        Vector2 velocityNeeded = targetVelocity - Vector2.Dot(_monster.RigidBody2D.velocity, moveDirection) * moveDirection;   // ������ ���� �����̱� ���� ����
        Vector2 moveForce = velocityNeeded * _monster.monsterData.Acceleration;

        Debug.DrawRay(transform.position, moveDirection, Color.cyan);

        _monster.RigidBody2D.AddForce(moveForce);
    }

    // �ִϸ��̼� �̺�Ʈ�� ���
    public void GroundJumpping()
    {
        Vector2 forceVector = new Vector2(_monster.monsterData.JumpForce.x * _monster.RecentDir, _monster.monsterData.JumpForce.y);
        _monster.RigidBody2D.AddForce(forceVector, ForceMode2D.Impulse);
    }
}
