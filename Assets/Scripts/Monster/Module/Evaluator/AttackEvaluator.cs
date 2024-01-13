using System.Collections;
using UnityEngine;

/// <summary>
/// 공격 판정기
/// </summary>
public class AttackEvaluator : Evaluator
{
    public override bool IsTargetWithinRange()
    {
        if (IsDuringCoolTime || !IsUsable)
            return false;

        // check target within range
        Collider2D targetCollider = Physics2D.OverlapBox(_checkCollider.transform.position, _checkCollider.bounds.size, 0f, _targetLayer);
        if (targetCollider)
        {
            // check player
            PlayerBehaviour player = targetCollider.GetComponent<PlayerBehaviour>();
            if (player && !player.IsDead)
                return true;
        }

        return false;
    }

    public override IEnumerator CoolTimeCoroutine()
    {
        var coolTimeCoroutine = base.CoolTimeCoroutine();
        yield return coolTimeCoroutine;
    }

    public override void StartCoolTimeCoroutine()
    {
        base.StartCoolTimeCoroutine();
    }
}
