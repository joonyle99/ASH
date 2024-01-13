using UnityEngine;

public class FloatingChaseEvaluator : Evaluator
{
    public Transform TargetTrans { get; private set; }

    public override bool IsTargetWithinRange()
    {
        if (IsDuringCoolTime || !IsUsable)
            return false;

        // Detect Collider
        Collider2D targetCollider = Physics2D.OverlapBox(_checkCollider.transform.position, _checkCollider.bounds.size, 0f, _targetLayer);
        if (targetCollider)
        {
            // Check Player
            PlayerBehaviour player = targetCollider.GetComponent<PlayerBehaviour>();
            if (player && !player.IsDead)
            {
                SetTargetTrans(player.transform);
                return true;
            }
        }

        return false;
    }
    public void SetTargetTrans(Transform trans)
    {
        TargetTrans = trans;
    }
}
