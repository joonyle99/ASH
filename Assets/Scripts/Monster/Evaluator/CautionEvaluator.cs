using UnityEngine;

/// <summary>
/// 경계 태세 판독기
/// </summary>
public class CautionEvaluator : Evaluator
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
}
