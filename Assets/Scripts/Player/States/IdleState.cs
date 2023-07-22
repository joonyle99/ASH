using UnityEngine;

public class IdleState : PlayerState
{
    /// <summary>
    /// ������ ������ �̲������� �ʵ��� �ϴ� ������
    /// </summary>
    Vector2 _groundNormal;          // ���� ��������
    Vector3 _groundHitPoint;        // ���� Hit Point
    float _forcePower = 100f;       // �Ʒ��� �����ִ� ��
    public float _angle = 0f;

    protected override void OnEnter()
    {

    }

    protected override void OnUpdate()
    {
        // ���� ��������
        _groundNormal = Player.GroundHit.normal;

        // ���� Hit Point
        _groundHitPoint = Player.GroundHit.point;

        // Run State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) != 0)
        {
            ChangeState<RunState>();
            return;
        }

        _angle = Vector3.Angle(_groundNormal, Player.PlayerLookDir);
        if (Mathf.Abs(90 - _angle) > 10f)
            Player.Rigidbody.velocity = new Vector2(-_groundNormal.x, -_groundNormal.y) * _forcePower * Time.deltaTime;
    }

    protected override void OnExit()
    {

    }

    /// <summary>
    /// ���� �������� �ð�ȭ
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_groundHitPoint, _groundHitPoint + new Vector3(_groundNormal.x, _groundNormal.y, 0f));
    }
}