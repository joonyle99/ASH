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
    private float _orginGravity;

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
            // 대쉬 실행
            // 가속도 때문에 Update()에서 속도를 계속해서 고정해준다.
            Player.Rigidbody.velocity = _dashDir * _dashSpeed;

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
        FinishDash();

        Player.Animator.SetBool("IsDash", _isDashing);
    }

    private void ExcuteDash()
    {
        // 기존 중력 저장
        _orginGravity = Player.Rigidbody.gravityScale;

        // 대쉬 실행 시 속성 설정
        _isDashing = true;
        Player.CanDash = false;

        // 중력 0으로 설정
        Player.Rigidbody.gravityScale = 0;

        // Dash를 시작한 시간
        _timeStartedDash = Time.time;
    }

    private void FinishDash()
    {
        // 대쉬 종료 시 속성 설정
        _isDashing = false;

        // 대쉬가 끝나는 순간의 시간
        _timeEndeddDash = Time.time;

        // 기존 중력으로 되돌리기
        Player.Rigidbody.gravityScale = _orginGravity;
    }

    public void SetDashDir(float xDirection)
    {
        // 대쉬 방향 설정
        _dashDir = new Vector2(xDirection, 0f).normalized;
    }
}