using UnityEngine;

public class IdleState : PlayerState
{
    [Header("Idle Setting")]

    [Space]

    [SerializeField] float _belowForce = 25f;       // �Ʒ��� �����ִ� ��

    Vector2 _groundNormal;                          // ���� ��������
    Vector3 _groundHitPoint;                        // ���� Hit Point

    protected override void OnEnter()
    {
        // ���� ��������
        _groundNormal = Player.GroundHit.normal;

        // ���� Hit Point
        _groundHitPoint = Player.GroundHit.point;
    }

    protected override void OnUpdate()
    {
        // Run State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) != 0)
        {
            ChangeState<RunState>();
            return;
        }

        // �÷��̾�� �� ������ ����
        // float _angle = Vector3.Angle(_groundNormal, Player.PlayerLookDir);

        // ������ ������ �̲��� ����
        // if (Mathf.Abs(90f - _angle) > 5f)

        // �̵��� ���߸� �̲��� ����
        // ������ ������ ȿ����
        // �ٵ� �׳� �������� Ű��� �Ǵ°� �ƴϾ�..?
        // �׷��� �ϸ� ������ ���� �������� ���ϳ� ~
        Player.Rigidbody.AddForce(-_groundNormal * _belowForce);
    }

    protected override void OnExit()
    {

    }

    private void OnDrawGizmosSelected()
    {
        // ���� �������� �׸���
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_groundHitPoint, _groundHitPoint + new Vector3(_groundNormal.x, _groundNormal.y, 0f));
    }
}