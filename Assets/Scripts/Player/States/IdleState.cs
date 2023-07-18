using UnityEngine;

public class IdleState : PlayerState
{
    Vector2 _groundNormal;
    Vector3 _tempGroundHitPoint;
    float _forcePower = 100f;

    protected override void OnEnter()
    {
        // �÷��̾ ���� ������� ���� ���
        if (!Player.GroundHit)
            return;

        // ���� ��������
        _groundNormal = Player.GroundHit.normal;

        // ���� raycast hit ��ġ��
        _tempGroundHitPoint = Player.GroundHit.point;
    }

    protected override void OnUpdate()
    {
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) != 0)
        {
            ChangeState<RunState>();
            return;
        }

        // ������ �̲������� ���� (�������� ó�� X)
        float angle = Vector3.Angle(_groundNormal, Player.PlayerLookDir);
        if (Mathf.Abs(90 - angle) > 5f)
            Player.Rigidbody.velocity = new Vector2((-1) * _groundNormal.x, (-1) * _groundNormal.y) * _forcePower * Time.deltaTime;
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
        Gizmos.DrawLine(_tempGroundHitPoint, _tempGroundHitPoint + new Vector3(_groundNormal.x, _groundNormal.y, 0f));
    }
}