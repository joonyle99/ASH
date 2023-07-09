using UnityEngine;

public class DashState : PlayerState
{
    [Header("Dash Setting")]
    [SerializeField] private float _dashSpeed = 20f;
    [SerializeField] private float _dashLength = 0.2f;
    [SerializeField] private float _coolTime = 0.3f;

    private bool _enableDash = true;
    private bool _dashing = false;
    private float _timeStartedDash;
    private float _timeEndeddDash;
    private Vector2 _dashDir;

    public bool EnableDash { get { return _enableDash; } set { _enableDash = value; } }
    public bool Dashing { get { return _dashing; } }
    public float TimeEndedDash { get { return _timeEndeddDash; } }
    public float CoolTime { get { return _coolTime; } }

    protected override void OnEnter()
    {
        //Debug.Log("Dash Enter");

        Player.Animator.SetBool("Dash", true);

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
                Player.ChangeState<InAirState>();
                return;
            }
        }
    }

    protected override void OnExit()
    {
        //Debug.Log("Dash Exit");

        Player.Animator.SetBool("Dash", false);
    }

    private void ExcuteDash()
    {
        _dashing = true;
        _enableDash = false;
        Player.Rigidbody.gravityScale = 0;                                      // 중력 0으로 설정
        _dashDir = new Vector2(Player.RawInputs.Movement.x, 0f).normalized;     // 대쉬 방향 설정

        Player.Rigidbody.velocity = _dashDir * _dashSpeed;                      // 대쉬 실행
        _timeStartedDash = Time.time;                                           // Dash를 시작한 시간을 기록
    }
}