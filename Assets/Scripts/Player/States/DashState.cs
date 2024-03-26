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
            // ���ӵ� ������ Update()���� �ӵ��� ����ؼ� �������ش�
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
        // ���� �߷� ����
        _orginGravity = Player.Rigidbody.gravityScale;

        // �뽬 ���� �� �Ӽ� ����
        IsDashing = true;
        Player.CanDash = false;

        // �뽬 ���� ����
        Player.IsGodMode = true;
        // Player.StartSuperArmorFlash();

        // �߷� 0���� ����
        Player.Rigidbody.gravityScale = 0f;
    }

    private void FinishDash()
    {
        // �뽬 ���� �� �Ӽ� ����
        IsDashing = false;

        // ���� ����
        Player.IsGodMode = false;

        // ���� �߷����� �ǵ�����
        Player.Rigidbody.gravityScale = _orginGravity;
    }

    private IEnumerator DashCoolDown()
    {
        yield return new WaitForSeconds(_targetDashCoolTime);
        Player.CanDash = true;
    }

    public void SetDashDir(float xDirection)
    {
        // �뽬 ���� ����
        _dashDir = new Vector2(xDirection, 0f).normalized;
    }
}