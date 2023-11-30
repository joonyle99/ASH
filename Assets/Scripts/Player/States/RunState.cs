using UnityEngine;
using UnityEngine.EventSystems;

public class RunState : PlayerState
{
    [Header("Run Setting")]

    [Space]

    [SerializeField] bool _isPlayerOnSlope;
    [SerializeField] Vector2 _curVelocity;
    [SerializeField] Vector2 _moveDir;
    [SerializeField] float _moveDirLength = 2.5f;
    [SerializeField] float _moveForce;
    [SerializeField] float _angleWithGround;
    [SerializeField] float _maxSpeed = 8f;
    [SerializeField] float _acceleration = 15f;
    [SerializeField] float _decceleration = 15f;

    protected override void OnEnter()
    {
        Player.Animator.SetBool("IsRun", true);
    }

    protected override void OnUpdate()
    {
        // Change to Idle State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) == 0)
        {
            ChangeState<IdleState>();
            return;
        }

        // Change to Dash State
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Player.CanDash && Mathf.RoundToInt(Player.RawInputs.Movement.x) != 0)
            {
                ChangeState<DashState>();
                return;
            }
        }

        // Change to Wall Grab State
        if (Player.IsTouchedWall && Player.IsLookForceSync && Mathf.RoundToInt(Player.RawInputs.Movement.y) > 0)
        {
            ChangeState<WallGrabState>();
            return;
        }

        // 현재 Velocity 참조
        _curVelocity = Player.Rigidbody.velocity;

        // -------------------------------------- //
        //              이동 속도 설정             //
        // -------------------------------------- //

        // 목표 속도 계산
        float targetSpeed = Player.RawInputs.Movement.x * _maxSpeed;

        // 가해야 할 힘의 양을 구하기 위한 속도 차이 계산
        float speedDif = targetSpeed - Player.Rigidbody.velocity.x;

        // 움직임 입력이 있는 경우
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _acceleration : _decceleration;

        // 이동 시키는 힘을 구한다.
        _moveForce = speedDif * accelRate;

        // -------------------------------------- //
        //              이동 방향 설정             //
        // -------------------------------------- //

        // 감지한 땅
        RaycastHit2D ground = Player.GroundHitWithRayCast;

        // 감지한 땅과의 각도
        _angleWithGround = Vector2.Angle(ground.normal, Player.PlayerLookDir2D);

        // 경사면에 있는지 체크
        _isPlayerOnSlope = Mathf.Abs(90f - _angleWithGround) > 10f;

        // 평지와 경사면에 따라 MoveDir 계산이 달라진다.
        if (_isPlayerOnSlope)
        {
            // 땅의 법선벡터와 플레이어가 바라보는 방향의 내적을 구한다
            float dot = Vector2.Dot(ground.normal, Player.PlayerLookDir2D);

            // 내적 > 0 == 예각 == 내리막길
            if (dot > 0)
            {
                // Debug.Log("LookDir과 GroundNormal 사이는 예각, 플레이어는 내리막길 가고있다.");

                // 내리막길을 오른쪽으로 간다
                if (Player.PlayerLookDir2D.x > 0f)
                {
                    // Perpendicular()로 구한 벡터는 시계 반대 방향으로 항상 90도 회전한 벡터
                    _moveDir = (-1) * Vector2.Perpendicular(ground.normal).normalized;
                }
                // 내리막길을 왼쪽으로 간다
                else
                    _moveDir = Vector2.Perpendicular(ground.normal).normalized;
            }
            // 작으면 둔각
            // 내적 < 0 == 둔각 == 오르막길
            else
            {
                // Debug.Log("LookDir과 GroundNormal 사이는 둔각, 플레이어는 오르막길 가고있다.");

                // 오르막길을 오른쪽으로 간다
                if (Player.PlayerLookDir2D.x > 0f)
                    _moveDir = (-1) * Vector2.Perpendicular(ground.normal).normalized;
                // 오르막길을 왼쪽으로 간다
                else
                    _moveDir = Vector2.Perpendicular(ground.normal).normalized;
            }
        }
        else
        {
            _moveDir = Player.PlayerLookDir2D;
        }
    }
    protected override void OnFixedUpdate()
    {
        /*
        if (_isPlayerOnSlope)
        {
            // 플레이어 이동에 힘을 적용시킨다
            Player.Rigidbody.AddForce(_moveDir * Mathf.Abs(_moveForce));
        }
        else
        {
            // 플레이어 이동에 힘을 적용시킨다
            Player.Rigidbody.AddForce(_moveDir * Mathf.Abs(_moveForce));
        }
        */

        // 플레이어 이동에 힘을 적용시킨다
        Player.Rigidbody.AddForce(Vector2.right * _moveForce);
    }

    protected override void OnExit()
    {
        Player.Animator.SetBool("IsRun", false);
    }

    private void OnDrawGizmosSelected()
    {
        // 플레이어가 이동하는 방향
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(_moveDir.x, _moveDir.y, 0) * _moveDirLength);
    }
}