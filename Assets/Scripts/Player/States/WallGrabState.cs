using UnityEngine;

/// <summary>
/// wall grab state�� ����
/// wall slide / wall climb state�� �� �� �ִ�
/// �� wall state �� ���� �⺻�� ����
/// </summary>
public class WallGrabState : WallState
{
    private float _paddingValue = 0.5f;
    private float _prevGravity;

    protected override void OnEnter()
    {
        // Wall Normal, Perpendicular ������ ����
        base.OnEnter();

        _prevGravity = Player.Rigidbody.gravityScale;
        Player.Rigidbody.gravityScale = 0f;
        Player.Rigidbody.velocity = Vector2.zero;

        Animator.SetBool("IsGrab", true);

        transform.position = new Vector3(wallHitPos.x - _paddingValue * Player.PlayerLookDir2D.x, transform.position.y, transform.position.z);
    }

    protected override void OnUpdate()
    {
        // Wall Climb State
        if (Player.IsMoveYKey)
        {
            ChangeState<WallClimbState>();
            return;
        }
    }
    protected override void OnExit()
    {
        Player.Rigidbody.gravityScale = _prevGravity;

        Animator.SetBool("IsGrab", false);
    }
}
