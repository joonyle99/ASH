using System;
using UnityEngine;

public class GroundChaseEvaluator : MonoBehaviour
{
    [Header("Ground Chase Evaluator")]
    [Space]

    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private BoxCollider2D _chaseCheckCollider;
    [SerializeField] private int _chaseDir = 1;
    public int ChaseDir
    {
        get => _chaseDir;
        private set => _chaseDir = value;
    }
    [SerializeField] private bool _isChasable;
    public bool IsChasable
    {
        get => _isChasable;
        set => _isChasable = value;
    }

    private Vector2 _chaseCheckBoxSize;
    public Transform TargetTrans { get; private set; }

    private void Awake()
    {
        _chaseCheckBoxSize = _chaseCheckCollider.bounds.size;
    }

    public bool IsTargetWithinChaseRange()
    {
        if (!IsChasable)
            return false;

        // Detect Collider
        Collider2D targetCollider = Physics2D.OverlapBox(transform.position, _chaseCheckBoxSize, 0f, _targetLayer);
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

                return true;
            }
        }

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
