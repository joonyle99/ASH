using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingChaseEvaluator : MonoBehaviour
{
    [Header("Floating Chase Evaluator")]
    [Space]

    [SerializeField] private LayerMask _targetLayer;

    [SerializeField] private BoxCollider2D _chaseArea;
    private Bounds _chaseBounds;

    public Transform TargetTrans { get; private set; }

    // Test Code
    [SerializeField] private GameObject checkPrefab;
    private GameObject _chaseTargetPoint;

    void Awake()
    {
        _chaseBounds = _chaseArea.bounds;
    }

    public bool IsTargetWithinChaseRange()
    {
        // Detect Collider
        Collider2D targetCollider = Physics2D.OverlapBox(_chaseArea.transform.position, _chaseBounds.size, 0f, _targetLayer);
        if (targetCollider)
        {
            // Check Player
            PlayerBehaviour player = targetCollider.GetComponent<PlayerBehaviour>();
            if (player && !player.IsDead)
            {
                // Set Destination
                SetTargetTrans(player.transform);

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

    private void OnDrawGizmosSelected()
    {
        // 추격 탐지 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_chaseArea.transform.position, _chaseBounds.size);
    }
}
