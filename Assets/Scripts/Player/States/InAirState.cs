using UnityEngine;

public class InAirState : PlayerState
{
    [Header("InAir Setting")]
    [SerializeField] float _inAirSpeed = 7f;
    [SerializeField] float _fastDropThreshhold = 7f;
    [SerializeField] float _fastDropPower = 1f;
    [SerializeField] float _maxDropSpeed = -80f;

    PlayerJumpController _jumpController;

    protected override void OnEnter()
    {
        //Debug.Log("InAir Enter");

        _jumpController = GetComponent<PlayerJumpController>();
    }

    protected override void OnUpdate()
    {
        // wall jump를 했을 때,, 다른 Target Velocity
        if(Player.IsWallJump)
        {
            // wall jump는 "공중에서 이동 불가능"
            Vector2 targetVelocity = new Vector2(Player.Rigidbody.velocity.x, Player.Rigidbody.velocity.y);
            Player.Rigidbody.velocity = targetVelocity;
        }
        // jump
        else
        {
            // 좌우 입력
            float xInput = Player.SmoothedInputs.Movement.x;
            Vector2 targetVelocity = new Vector2(xInput * _inAirSpeed, Player.Rigidbody.velocity.y);
            Player.Rigidbody.velocity = targetVelocity;
        }

        if (Player.IsGrounded)
        {
            ChangeState<IdleState>();
            return;
        }

        // // 한계점 지나면 더 빨리 떨어짐
        if (Player.Rigidbody.velocity.y < _fastDropThreshhold)
        {
            // 떨어지는 속도에 최대값 부여
            if (Player.Rigidbody.velocity.y < _maxDropSpeed)
                Player.Rigidbody.velocity = Vector2.up * _maxDropSpeed;
            else
                Player.Rigidbody.velocity += _fastDropPower * Physics2D.gravity * Time.deltaTime;
        }

        // Debug.Log("player velocity y" + Player.Rigidbody.velocity.y);

        // Wall Grab State
        if (Player.IsTouchedWall && (Player.RecentDir == Mathf.RoundToInt(Player.RawInputs.Movement.x)))
        {
            ChangeState<WallGrabState>();
            return;
        }

        //// Wall Slide State
        //if (Player.IsTouchedWall && Player.Rigidbody.velocity.y < 0)
        //{
        //    ChangeState<WallSlideState>();
        //    return;
        //}
    }

    protected override void OnExit()
    {
        //Debug.Log("InAir Exit");

        Player.IsWallJump = false;
    }
}