using UnityEngine;

public class IdleState : PlayerState
{
    [Header("Idle Setting")]

    [Space]

    [SerializeField] float _belowForce = 6000f;       // �Ʒ��� �����ִ� ��

    Vector2 _groundNormal;                            // ���� ��������
    Vector3 _groundHitPoint;                          // ���� Hit Point

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
        float _angle = Vector2.Angle(_groundNormal, Player.PlayerLookDir);

        // ������ ������ �̲��� ����
        if (Mathf.Abs(90f - _angle) > 10f)
        {
            // Debug.Log("������ ���Դϴ�");
            Player.Rigidbody.AddForce(-_groundNormal * _belowForce * Time.deltaTime);
        }
        else
        {
            // Debug.Log("������ ���Դϴ�");
            Player.Rigidbody.AddForce(-_groundNormal * _belowForce / 3f * Time.deltaTime);
        }


    }
    protected override void OnFixedUpdate()
    {

    }

    protected override void OnExit()
    {

    }

    // private void OnDrawGizmosSelected()
    private void OnDrawGizmos()
    {
        // ���� �������� �׸���
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_groundHitPoint, _groundHitPoint + new Vector3(_groundNormal.x, _groundNormal.y, 0f));
    }
}