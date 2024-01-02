using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseEvaluator : MonoBehaviour
{
    [Header("Chase Evaluator")]
    [Space]

    [SerializeField] private LayerMask _targetLayer;

    [SerializeField] private BoxCollider2D _chaseArea;
    private Bounds _chaseBounds;

    [SerializeField] private Transform _targetTrans;
    public Transform TargetTrans { get { return _targetTrans; } }

    [SerializeField] private GameObject checkPrefab;
    [SerializeField] private GameObject targetPoint;

    void Awake()
    {
        _chaseBounds = _chaseArea.bounds;
    }

    public bool IsTargetWithinChaseRange()
    {
        // Detect Collider
        Collider2D targetCollider = Physics2D.OverlapBox(_chaseArea.transform.position, _chaseBounds.size, 0f, _targetLayer);
        if (targetCollider != null)
        {
            // Check Player
            PlayerBehaviour player = targetCollider.GetComponent<PlayerBehaviour>();
            if (player != null && !player.IsDead)
            {
                // Set Destination
                SetTargetTrans(player.transform);

                // Create Debug Object
                if (!targetPoint)
                    targetPoint = Instantiate(checkPrefab, _targetTrans.position, Quaternion.identity);
                else
                    targetPoint.transform.position = player.transform.position;

                return true;
            }
        }

        // Delete Debug Object
        if (targetPoint)
            Destroy(targetPoint);

        return false;
    }

    public void SetTargetTrans(Transform trans)
    {
        _targetTrans = trans;
    }

    private void OnDrawGizmosSelected()
    {
        // 추격 탐지 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_chaseArea.transform.position, _chaseBounds.size);
    }
}
