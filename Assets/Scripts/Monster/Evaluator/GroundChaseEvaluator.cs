using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// ���� ���Ͱ� ����� �߰� ���� �ȿ� ���Դ��� �Ǵ��ϰ� ������ ��ȯ�ϴ� Ŭ����
/// </summary>
public class GroundChaseEvaluator : Evaluator
{
    [field: Header("Ground Chase Evaluator")]
    [field: Space]
    [field: SerializeField]
    public int ChaseDir
    {
        get;
        private set;
    } = 1;
    [field: SerializeField]
    public float MaxChaseDistance
    {
        get;
        private set;
    } = 3;
    [field: SerializeField]
    public bool IsChasing
    {
        get;
        private set;
    }
    [field: SerializeField]
    public bool IsTooClose
    {
        get;
        set;
    }

    public override void Awake()
    {
        base.Awake();

        EvaluationEvent -= SetChaseDir;
        EvaluationEvent += SetChaseDir;
    }

    protected override IEnumerator WaitEventCoroutine()
    {
        Debug.Log("start =====> curstom cool time coroutine");

        IsDuringCoolTime = true;

        // TODO: �÷��̾�� ������ ���� ������ ��ȯ�� �� �ֵ���
        // 1. ��� ������ ���°� �ȴ�
        // 2. �÷��̾� �������� �߰� ������ �����Ѵ�
        // 3. �÷��̾� �������� �̵��� ������ ����� �����Ѵ� (��Ÿ���� �Ǵ�)
        // 4. �ٽ� 1�� ���ư���

        var player = SceneContext.Current.Player;
        var dir = Mathf.Sign(player.transform.position.x - transform.position.x);
        var targetPosX = player.transform.position.x + dir * 30f;

        Vector3 startPoint = new Vector3(targetPosX, transform.position.y, transform.position.z);
        Vector3 endPoint = new Vector3(targetPosX, transform.position.y + 5f, transform.position.z);
        Debug.DrawLine(startPoint, endPoint, Color.cyan, 1f);

        yield return new WaitUntil(() => Mathf.Abs(transform.position.x - targetPosX) < 0.5f);

        Debug.Log("end =====> curstom cool time coroutine");

        IsDuringCoolTime = false;
    }

    public override Collider2D IsTargetWithinRange()
    {
        var hasChaseTarget = base.IsTargetWithinRange();

        IsChasing = hasChaseTarget;
        return hasChaseTarget;
    }

    public void SetChaseDir(Vector3 targetPoint)
    {
        ChaseDir = Math.Sign(targetPoint.x - transform.position.x);
    }
}
