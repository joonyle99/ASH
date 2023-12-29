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
            // �뽬 ����
            // ���ӵ� ������ Update()���� �ӵ��� ����ؼ� �������ش�.
            Player.Rigidbody.velocity = _dashDir * _dashSpeed;

            _elapsedDashTime += Time.deltaTime;

            // �뽬�� ������ ����
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
        // ���� �߷� ����
        _orginGravity = Player.Rigidbody.gravityScale;

        // �뽬 ���� �� �Ӽ� ����
        _isDashing = true;
        Player.CanDash = false;

        // �߷� 0���� ����
        Player.Rigidbody.gravityScale = 0f;
    }

    private void FinishDash()
    {
        // �뽬 ���� �� �Ӽ� ����
        _isDashing = false;

        // ���� �߷����� �ǵ�����
        Player.Rigidbody.gravityScale = _orginGravity;
    }

    public void SetDashDir(float xDirection)
    {
        // �뽬 ���� ����
        _dashDir = new Vector2(xDirection, 0f).normalized;
    }
}