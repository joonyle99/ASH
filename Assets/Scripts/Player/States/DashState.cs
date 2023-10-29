using UnityEngine;

public class DashState : PlayerState
{
    [Header("Dash Setting")]

    [Space]

    [Range(0f, 50f)] [SerializeField] float _dashSpeed = 20f;
    [Range(0f, 5f)] [SerializeField] float _dashLength = 0.2f;
    [Range(0f, 5f)] [SerializeField] float _coolTime = 0.3f;

    private Vector2 _dashDir;
    private bool _isDashing;
    private float _timeStartedDash;
    private float _timeEndeddDash;
    private float _oldGravity;

    public bool IsDashing { get { return _isDashing; } }
    public float TimeEndedDash { get { return _timeEndeddDash; } }
    public float CoolTime { get { return _coolTime; } }

    protected override void OnEnter()
    {
        Player.Animator.SetBool("IsDash", true);

        _oldGravity = Player.Rigidbody.gravityScale;

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

        _timeEndeddDash = Time.time;                                            // 대쉬가 끝나는 순간의 시간
        Player.Rigidbody.gravityScale = _oldGravity;
    }

    private void ExcuteDash()
    {
        Player.PlayerSound_SE_Dash();                                           // 대쉬 사운드 재생

        _isDashing = true;
        Player.CanDash = false;

        Player.Rigidbody.gravityScale = 0;                                      // 중력 0으로 설정
        _dashDir = new Vector2(Player.RawInputs.Movement.x, 0f).normalized;   // 대쉬 방향 설정

        Player.Rigidbody.velocity = _dashDir * _dashSpeed;                      // 대쉬 실행

        _timeStartedDash = Time.time;                                           // Dash를 시작한 시간
    }
}