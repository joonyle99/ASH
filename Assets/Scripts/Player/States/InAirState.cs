using UnityEngine;

public class InAirState : PlayerState, IAttackableState, IJumpableState
{
    [Header("InAir Setting")]

    [Space]

    [SerializeField] float _inAirMoveAcceleration = 30f;          // 공중에서 좌우로 움직이는 스피드
    [SerializeField] float _maxInAirMoveSpeed = 6f;         // 공중에서 좌우로 움직이는 최대 스피드
    [SerializeField] float _fastDropThreshhold = 4f;    // 빨리 떨어지기 시작하는 높이
    [SerializeField] float _fastDropPower = 1.2f;        // 빨리 떨어지는 힘
    [SerializeField] float _maxDropSpeed = 60f;        // 떨어지는 속도 최대값

    public bool IsInAir;

    protected override bool OnEnter()
    {
        IsInAir = true;
        Player.Animator.SetBool("IsInAir", IsInAir);

        // Debug.Log($"Animator Current State: {Player.Animator.GetCurrentAnimatorStateInfo(0).IsName("In Air")}");
        // Debug.Log($"InAirState OnEnter() IsInAir: {Player.Animator.GetBool("IsInAir")} (IsInAir variable: {IsInAir})");

        return true;
    }

    protected override bool OnUpdate()
    {
        // Change to Idle State
        if (Player.IsGrounded == true)
        {
            // 공중에서 바닥에 닿으면 나는 사운드
            Player.PlaySound_SE_Jump_02();

            ChangeState<IdleState>();
            return true;
        }

        // Change to Dash State
        if (InputManager.Instance.State.DashKey.KeyDown)
        {
            if (Player.CanDash && Player.IsMoveXKey)
            {
                ChangeState<DashState>();
                return true;
            }
        }

        // Change to Wall Grab State
        if (Player.IsTouchedWall == true)
        {
            if (Player.IsClimbable)
            {
                if (Player.IsDirSync && Player.IsMoveUpKey)
                {
                    ChangeState<WallGrabState>();
                    return true;
                }
            }
        }

        // 임시 해결책
        Player.Animator.SetBool("IsInAir", IsInAir);
        // Debug.Log($"InAirState OnUpdate() IsInAir: {Player.Animator.GetBool("IsInAir")} (IsInAir variable: {IsInAir})");

        // Wall Jump에서 In Air State로 넘어온 경우
        if (Player.IsClimbJump)
        {
            // 플레이어의 velocity를 그대로 가지고 간다
            Player.Rigidbody.velocity = new Vector2(Player.Rigidbody.velocity.x, Player.Rigidbody.velocity.y);
            Player.IsClimbJump = false;
        }
        // Basic Jump에서 In Air State로 넘어온 경우
        else
        {
            // 공중에서의 최대 이동속도를 제한한다
            if (Mathf.Abs(Player.Rigidbody.velocity.x) > _maxInAirMoveSpeed)
                Player.Rigidbody.velocity = new Vector2(Mathf.Sign(Player.Rigidbody.velocity.x) * _maxInAirMoveSpeed, Player.Rigidbody.velocity.y);
        }

        // 한계점 지나면 더 빨리 떨어짐
        if (Player.Rigidbody.velocity.y < _fastDropThreshhold)
        {
            // 떨어지는 속도에 최대값 부여
            if (Player.Rigidbody.velocity.y < (-1) * _maxDropSpeed)
                Player.Rigidbody.velocity = new Vector2(Player.Rigidbody.velocity.x, (-1) * _maxDropSpeed);
        }

        return true;
    }
    protected override bool OnFixedUpdate()
    {
        // Basic Jump에서 In Air State로 넘어온 경우
        if (!Player.IsClimbJump)
        {
            // 공중에서 좌우로 움직일 수 있다.
            Player.Rigidbody.AddForce(Vector2.right * Player.RawInputs.Movement.x * _inAirMoveAcceleration);
        }

        // 한계점 지나면 더 빨리 떨어짐
        if (Player.Rigidbody.velocity.y < _fastDropThreshhold)
        {
            // 가속 낙하
            Player.Rigidbody.AddForce(_fastDropPower * Physics2D.gravity);
        }

        return true;
    }

    protected override bool OnExit()
    {
        IsInAir = false;
        Player.Animator.SetBool("IsInAir", IsInAir);

        // Debug.Log($"InAirState OnExit() IsInAir: {Player.Animator.GetBool("IsInAir")} (IsInAir variable: {IsInAir})");

        return true;
    }
}