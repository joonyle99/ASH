using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 지상 몬스터가 대상이 추격 범위 안에 들어왔는지 판단하고 방향을 전환하는 클래스
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

        // TODO: 플레이어에게 돌진할 때만 방향을 전환할 수 있도록
        // 1. 사용 가능한 상태가 된다
        // 2. 플레이어 방향으로 추격 방향을 설정한다
        // 3. 플레이어 뒷편으로 이동할 때까지 사용을 제한한다 (쿨타임을 건다)
        // 4. 다시 1로 돌아간다

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
