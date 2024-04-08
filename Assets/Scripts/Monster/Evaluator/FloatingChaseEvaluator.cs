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

        evaluationEvent -= ChangeTarget;
        evaluationEvent += ChangeTarget;
    }

    /// <summary>
    /// NavMesh�� Ÿ���� �����Ѵ�
    /// </summary>
    /// <param name="targetPoint"></param>
    private void ChangeTarget(Vector3 targetPoint)
    {
        // �߰� Ÿ���� �����Ѵ�
        TargetPosition = targetPoint;
    }
}
