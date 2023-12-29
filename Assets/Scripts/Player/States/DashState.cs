using System.Collections;
using UnityEngine;

public class DashState : PlayerState
{
    [Header("Dash Setting")]

    [Space]

    [SerializeField] float _dashSpeed = 20f;
    [SerializeField] float _targetDashTime = 0.2f;
    [SerializeField] float _elapsedDashTime;

    private Vector2 _dashDir;
    private bool _isDashing;
    private float _orginGravity;

    public bool IsDashing { get { return _isDashing; } }

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

            _elapsedDashTime += Time.deltaTime;

            // 대쉬가 끝나는 조건
            if (_elapsedDashTime > _targetDashTime)
            {
                _elapsedDashTime = 0f;
                Player.ChangeState<InAirState>();
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
        Player.Rigidbody.gravityScale = 0f;
    }

    private void FinishDash()
    {
        // 대쉬 종료 시 속성 설정
        _isDashing = false;

        // 기존 중력으로 되돌리기
        Player.Rigidbody.gravityScale = _orginGravity;
    }

    public void SetDashDir(float xDirection)
    {
        // 대쉬 방향 설정
        _dashDir = new Vector2(xDirection, 0f).normalized;
    }
}