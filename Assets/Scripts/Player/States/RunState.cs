using UnityEngine;

public class RunState : PlayerState
{
    [Header("Run Setting")]

    [Space]

    [Range(0f, 30f)] [SerializeField] float _runSpeed = 7f;

    protected override void OnEnter()
    {
        Player.Animator.SetBool("IsRun", true);
    }

    protected override void OnUpdate()
    {
        // �÷��̾� �̵�
        float xInput = Player.SmoothedInputs.Movement.x;
        Player.Rigidbody.velocity = new Vector2(xInput * _runSpeed, Player.Rigidbody.velocity.y);

        // TODO : �÷��̾� �̵��� AddForce�� ����
        // Player.Rigidbody.AddForce(new Vector2(xInput * _runSpeed, Player.Rigidbody.velocity.y));

        // Idle State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) == 0)
        {
            ChangeState<IdleState>();
            return;
        }

        // Dash State
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Player.CanDash && Mathf.RoundToInt(Player.RawInputs.Movement.x) != 0)
            {
                ChangeState<DashState>();
                return;
            }
        }

        // Wall Grab State
        if (Player.IsTouchedWall && (Player.RecentDir == Mathf.RoundToInt(Player.RawInputs.Movement.x)) && Mathf.RoundToInt(Player.RawInputs.Movement.y) > 0)
        {
            ChangeState<WallGrabState>();
            return;
        }
    }

    protected override void OnExit()
    {
        Player.Animator.SetBool("IsRun", false);
    }

}
