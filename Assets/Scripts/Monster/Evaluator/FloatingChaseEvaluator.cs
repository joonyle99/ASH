using UnityEngine;

/// <summary>
/// ���� ���Ͱ� ����� �߰� ���� �ȿ� ���Դ��� �ϴ� �Ǵ��ϰ� Ÿ���� �����ϴ� Ŭ����
/// </summary>
public class FloatingChaseEvaluator : Evaluator
{
    public Vector3 TargetPosition { get; private set; }

    public override void Awake()
    {
        base.Awake();

        // Ÿ�� ���� �� �ڵ����� �߻��ϴ� ��� �߰�
        customEvaluationEvent -= SetTarget;
        customEvaluationEvent += SetTarget;
    }

    public override Collider2D IsTargetWithinRange()
    {
        return base.IsTargetWithinRange();
    }

    /// <summary>
    /// NavMesh�� Ÿ���� �����Ѵ�
    /// </summary>
    /// <param name="targetPoint"></param>
    private void SetTarget(Vector3 targetPoint)
    {
        // �߰� Ÿ���� �����Ѵ�
        TargetPosition = targetPoint;
    }
}
