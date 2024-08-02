using UnityEngine;
using joonyle99;

public sealed class GroundActionAreaDataSender : ActionAreaDataSender
{
    [Header("Ground ActionArea Data Sender")]
    [Space]

    private BoxCollider2D _patrolPointA;
    private BoxCollider2D _patrolPointB;

    protected override void Awake()
    {
        _patrolPointA = transform.GetChild(0).GetComponent<BoxCollider2D>();
        _patrolPointB = transform.GetChild(1).GetComponent<BoxCollider2D>();

        base.Awake();
    }

    public override void ExtractActionAreaInfo(out BoxCollider2D patrolPointA, out BoxCollider2D patrolPointB)
    {
        patrolPointA = _patrolPointA;
        patrolPointB = _patrolPointB;
    }
    public override void SetActionAreaPosition(Vector3 position1, Vector3 position2)
    {
        _patrolPointA.transform.position = position1;
        _patrolPointB.transform.position = position2;
    }
    public override void SetActionAreaScale(Vector3 scale1, Vector3 scale2)
    {
        _patrolPointA.transform.localScale = scale1;
        _patrolPointB.transform.localScale = scale2;
    }

    public override void UpdateActionAreaData()
    {
        // new�� Ŭ���� �Ǵ� ����ü�� ��ü�� �����ϰ� �޸𸮿� �Ҵ��ϱ� ���� ���ȴ�
        // struct�� value type�̱� ������ class�� heap�� �Ҵ�Ǵ� �Ͱ� �޸� stack�� �Ҵ�ȴ�.

        // ������ ���� ����
        Line3D respawnLine3D = new Line3D(_patrolPointA.bounds.center, _patrolPointB.bounds.center);
        Debug.DrawLine(respawnLine3D.pointA, respawnLine3D.pointB, Color.yellow, 3f);

        // receiver���� ������ ����
        receiver.SetGroundActionAreaData(_patrolPointA.transform.position, _patrolPointB.transform.position, respawnLine3D);
    }
}
