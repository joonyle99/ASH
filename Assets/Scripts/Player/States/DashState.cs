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
    private float _originGravity;
    private float _elapsedDashTime;

    protected override bool OnEnter()
    {
        Player.Animator.SetBool("IsDash", true);

        PreProcess();

        return true;
    }

    protected override bool OnUpdate()
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

        return true;
    }

    protected override bool OnFixedUpdate()
    {
        return true;
    }

    protected override bool OnExit()
    {
        PostProcess();

        StartCoroutine(DashCoolDown());

        Player.Animator.SetBool("IsDash", false);

        return true;
    }

    private void PreProcess()
    {
        {
            SceneContext.Current.Player.CanDash = false;
        }

        {
            Player.IsGodMode = true;
            _originGravity = Player.Rigidbody.gravityScale;
            Player.Rigidbody.gravityScale = 0f;
        }

        {
            _dashDir = new Vector2(Player.RawInputs.Movement.x, 0f).normalized;
        }

        {
            _elapsedDashTime = 0f;
        }
    }

    private void PostProcess()
    {
        {
            Player.IsGodMode = false;
            Player.Rigidbody.gravityScale = _originGravity;
        }

        {
            _dashDir = Vector2.zero;
            _originGravity = 0f;
        }

        {
            _elapsedDashTime = 0f;
        }
    }

    private IEnumerator DashCoolDown()
    {
        yield return new WaitForSeconds(_targetDashCoolTime);
        SceneContext.Current.Player.CanDash = true;
    }
}