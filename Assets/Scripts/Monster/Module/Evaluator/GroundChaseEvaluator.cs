using System;
using UnityEngine;

public class GroundChaseEvaluator : Evaluator
{
    [Header("Ground Chase Evaluator")]
    [Space]

    [SerializeField] private int _chaseDir = 1;
    public int ChaseDir
    {
        get => _chaseDir;
        private set => _chaseDir = value;
    }
    [SerializeField] private bool _isChasing;
    public bool IsChasing
    {
        get => _isChasing;
        private set => _isChasing = value;
    }
    [SerializeField] private bool _isChasable;
    public bool IsChasable
    {
        get => _isChasable;
        set => _isChasable = value;
    }

    public Transform TargetTrans { get; private set; }

    public override bool IsTargetWithinRange()
    {
        if (IsDuringCoolTime || !IsUsable)
        {
            IsChasing = false;
            return false;
        }

        if (!IsChasable)
        {
            IsChasing = false;
            return false;
        }

        // Detect Collider
        Collider2D targetCollider = Physics2D.OverlapBox(transform.position, _checkCollider.bounds.size, 0f, _targetLayer);
        if (targetCollider)
        {
            // Check Player
            PlayerBehaviour player = targetCollider.GetComponent<PlayerBehaviour>();
            if (player && !player.IsDead)
            {
                // Set Destination
                SetTargetTrans(player.transform);

                // Set Direction
                SetChaseDir(player.transform);

                IsChasing = true;
                return true;
            }
        }

        IsChasing = false;
        return false;
    }

    public void SetTargetTrans(Transform trans)
    {
        TargetTrans = trans;
    }

    public void SetChaseDir(Transform trans)
    {
        _chaseDir = Math.Sign(trans.position.x - transform.position.x);
    }
}
