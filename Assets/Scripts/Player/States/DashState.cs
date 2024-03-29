using System.Collections;
using UnityEngine;

public class DashState : PlayerState
{
    [Header("Dash Setting")]

    [Space]

    [SerializeField] float _dashSpeed = 20f;
    [SerializeField] float _targetDashDuration = 0.2f;
    [SerializeField] float _targetDashCoolTime = 0.7f;

    private Vector2 _dashDir;
    private float _orginGravity;
    private float _elapsedDashTime;
    public bool IsDashing { get; private set; }

    protected override bool OnEnter()
    {
        Player.Animator.SetBool("IsDash", true);

        ExcuteDash();

        return true;
    }

    protected override bool OnUpdate()
    {
        if (IsDashing)
        {
            // 가속도 때문에 Update()에서 속도를 계속해서 설정해준다
            Player.Rigidbody.velocity = _dashDir * _dashSpeed;

            _elapsedDashTime += Time.deltaTime;

            if (_elapsedDashTime > _targetDashDuration)
            {
                _elapsedDashTime = 0f;
                Player.ChangeState<InAirState>();
                return true;
            }
        }

        return true;
    }

    protected override bool OnFixedUpdate()
    {
        return true;
    }

    protected override bool OnExit()
    {
        FinishDash();

        StartCoroutine(DashCoolDown());

        Player.Animator.SetBool("IsDash", IsDashing);

        return true;
    }

    private void ExcuteDash()
    {
        // 기존 중력 저장
        _orginGravity = Player.Rigidbody.gravityScale;

        // 대쉬 실행 시 속성 설정
        IsDashing = true;
        Player.CanDash = false;

        // 대쉬 동안 무적
        Player.IsGodMode = true;
        // Player.StartSuperArmorFlash();

        // 중력 0으로 설정
        Player.Rigidbody.gravityScale = 0f;
    }

    private void FinishDash()
    {
        // 대쉬 종료 시 속성 설정
        IsDashing = false;

        // 무적 종료
        Player.IsGodMode = false;

        // 기존 중력으로 되돌리기
        Player.Rigidbody.gravityScale = _orginGravity;
    }

    private IEnumerator DashCoolDown()
    {
        yield return new WaitForSeconds(_targetDashCoolTime);
        Player.CanDash = true;
    }

    public void SetDashDir(float xDirection)
    {
        // 대쉬 방향 설정
        _dashDir = new Vector2(xDirection, 0f).normalized;
    }
}