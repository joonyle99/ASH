using UnityEngine;

public class DashState : PlayerState
{
    #region Dash

    [Header("Dash Setting")]
    [SerializeField] private float _dashSpeed = 20f;
    [SerializeField] private float _dashLength = 0.2f;
    [SerializeField] private float _coolTime = 0.3f;

    public bool EnableDash = true;

    private bool _dashing = false;
    private float _timeStartedDash;
    private float _timeEndeddDash;
    private Vector2 _dashDir;

    public bool Dashing { get { return _dashing; } }
    public float CoolTime { get { return _coolTime; } }
    public float TimeEndedDash { get { return _timeEndeddDash; } }

    #endregion
    
    protected override void OnEnter()
    {
        //Debug.Log("Dash Enter");

        ExcuteDash();
    }

    protected override void OnUpdate()
    {
        //Debug.Log("Update Dash");

        // Dashing
        if (_dashing)
        {
            // End Dash (대쉬가 끝나는 순간)
            if (Time.time >= _timeStartedDash + _dashLength)
            {
                _dashing = false;
                _timeEndeddDash = Time.time; // 대쉬가 끝나는 순간의 시간
                Player.Rigidbody.gravityScale = 5;
                //Player.Rigidbody.velocity = new Vector2(Player.Rigidbody.velocity.x, Player.Rigidbody.velocity.y);
                Player.ChangeState<InAirState>();
            }
        }
    }

    protected override void OnExit()
    {
        //Debug.Log("Dash Exit");
    }

    private void ExcuteDash()
    {
        _dashing = true;
        EnableDash = false;
        Player.Rigidbody.gravityScale = 0; // 중력 0으로 설정
        _dashDir = new Vector2(Player.RawInputs.Movement.x, 0).normalized; // 대쉬 방향 설정

        //if (_dashDir == Vector2.zero) // 키보드에 입력이 없이 대쉬를 하는 경우
        //    _dashDir = (Player.RecentDir.x < 0) ? Vector2.left : Vector2.right;

        Player.Rigidbody.velocity = _dashDir * _dashSpeed; // 대쉬 실행
        _timeStartedDash = Time.time; // Dash를 시작한 시간을 기록
    }
}