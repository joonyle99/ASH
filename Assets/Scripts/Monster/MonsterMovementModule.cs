using UnityEngine;

public class MonsterMovementModule : MonoBehaviour
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

        Vector2 groundNormal = _monster.GroundRayHit.normal;
        Vector2 moveDirection = _monster.RecentDir > 0
            ? (-1) * Vector2.Perpendicular(groundNormal)
            : Vector2.Perpendicular(groundNormal);

        Debug.DrawRay(_monster.GroundRayHit.point, groundNormal, Color.cyan);

        Vector2 targetVelocity = moveDirection * _monster.MoveSpeed;
        Vector2 velocityNeeded = targetVelocity - Vector2.Dot(_monster.Rigidbody.velocity, moveDirection) * moveDirection;   // ������ ���� �����̱� ���� ����
        Vector2 moveForce = velocityNeeded * _monster.Acceleration;

        Debug.DrawRay(transform.position, moveDirection, Color.cyan);

        _monster.Rigidbody.AddForce(moveForce);
    }

    // �ִϸ��̼� �̺�Ʈ�� ���
    public void GroundJumpping()
    {
        Vector2 forceVector = new Vector2(_monster.JumpForce.x * _monster.RecentDir, _monster.JumpForce.y);
        _monster.Rigidbody.AddForce(forceVector, ForceMode2D.Impulse);
    }

}
