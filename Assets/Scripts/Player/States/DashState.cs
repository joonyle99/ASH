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
            // �뽬 ����
            // ���ӵ� ������ Update()���� �ӵ��� ����ؼ� �������ش�.
            Player.Rigidbody.velocity = _dashDir * _dashSpeed;

            // �뽬�� ������ ����
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
        // ���� �߷� ����
        _orginGravity = Player.Rigidbody.gravityScale;

        // �뽬 ���� �� �Ӽ� ����
        _isDashing = true;
        Player.CanDash = false;

        // �߷� 0���� ����
        Player.Rigidbody.gravityScale = 0;

        // Dash�� ������ �ð�
        _timeStartedDash = Time.time;
    }

    private void FinishDash()
    {
        // �뽬 ���� �� �Ӽ� ����
        _isDashing = false;

        // �뽬�� ������ ������ �ð�
        _timeEndeddDash = Time.time;

        // ���� �߷����� �ǵ�����
        Player.Rigidbody.gravityScale = _orginGravity;
    }

    public void SetDashDir(float xDirection)
    {
        // �뽬 ���� ����
        _dashDir = new Vector2(xDirection, 0f).normalized;
    }
}