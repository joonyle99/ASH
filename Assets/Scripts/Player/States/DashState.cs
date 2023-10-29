using UnityEngine;

public class DashState : PlayerState
{
    [Header("Dash Setting")]

    [Space]

    [Range(0f, 50f)] [SerializeField] float _dashSpeed = 20f;
    [Range(0f, 5f)] [SerializeField] float _dashLength = 0.2f;
    [Range(0f, 5f)] [SerializeField] float _coolTime = 0.3f;

    Vector2 _dashDir;
    bool _isDashing;
    float _timeStartedDash;
    float _timeEndeddDash;

    public bool IsDashing { get { return _isDashing; } }
    public float TimeEndedDash { get { return _timeEndeddDash; } }
    public float CoolTime { get { return _coolTime; } }

    protected override void OnEnter()
    {
        Player.Animator.SetBool("IsDash", true);

        ExcuteDash();
    }

    protected override void OnUpdate()
    {
        if (_isDashing)
        {
            // 대쉬가 끝나는 조건
            if (Time.time >= _timeStartedDash + _dashLength)
            {
                Player.ChangeState<InAirState>();
                return;
            }
        }
    }
    protected override void OnFixedUpdate()
    {

    }

    protected override void OnExit()
    {
        _isDashing = false;
        Player.Animator.SetBool("IsDash", _isDashing);

        _timeEndeddDash = Time.time;        // 대쉬가 끝나는 순간의 시간
        Player.Rigidbody.gravityScale = 5;
    }

    private void ExcuteDash()
    {
        Player.PlayerSound_SE_Dash();

        _isDashing = true;
        Player.CanDash = false;

        Player.Rigidbody.gravityScale = 0;                                      // 중력 0으로 설정
        _dashDir = new Vector2(Player.RawInputs.Movement.x, 0f).normalized;   // 대쉬 방향 설정

        Player.Rigidbody.velocity = _dashDir * _dashSpeed;                      // 대쉬 실행

        // Debug.Log(Player.Rigidbody.velocity);

        _timeStartedDash = Time.time;                                           // Dash를 시작한 시간
    }
}