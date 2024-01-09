using System;
using UnityEngine;

public class GroundChaseEvaluator : MonoBehaviour
{
    [Header("Ground Chase Evaluator")]
    [Space]

    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private Vector2 _chaseBoxSize;

    public Transform TargetTrans { get; private set; }

    [SerializeField] private int _chaseDir = 1;
    public int ChaseDir
    {
        get => _chaseDir;
        private set => _chaseDir = value;
    }

        // Test Code
    [SerializeField] private GameObject checkPrefab;
    private GameObject _chaseTargetPoint;

    public bool IsTargetWithinChaseRange()
    {
        // Detect Collider
        Collider2D targetCollider = Physics2D.OverlapBox(transform.position, _chaseBoxSize, 0f, _targetLayer);
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

                // Create Debug Object
                if (!_chaseTargetPoint)
                {
                    _chaseTargetPoint = Instantiate(checkPrefab, TargetTrans.position, Quaternion.identity, transform.parent);
                    _chaseTargetPoint.name = "Chase Target Point";
                }
                else
                    _chaseTargetPoint.transform.position = player.transform.position;

                return true;
            }
        }

        // Delete Debug Object
        if (_chaseTargetPoint)
            Destroy(_chaseTargetPoint);

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(_chaseBoxSize.x, _chaseBoxSize.y, 0f));
    }
}
