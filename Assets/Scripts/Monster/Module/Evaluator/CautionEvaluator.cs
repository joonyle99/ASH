using System.Collections;
using UnityEngine;

public class CautionEvaluator : MonoBehaviour
{
    [Header("Caution Evaluator")]
    [Space]

    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private BoxCollider2D _cautionCheckBoxCollider;
    private Vector2 _cautionCheckBoxSize;

    private void Awake()
    {
        _cautionCheckBoxSize = _cautionCheckBoxCollider.bounds.size;
    }

    public bool IsTargetWithinCautionRange()
    {
        // 탐지 범위 안에 들어왔는지 확인
        Collider2D targetCollider = Physics2D.OverlapBox(_cautionCheckBoxCollider.transform.position, _cautionCheckBoxSize, 0f, _targetLayer);
        if (targetCollider)
        {
            // 플레이어인지 확인
            PlayerBehaviour player = targetCollider.GetComponent<PlayerBehaviour>();
            if (player && !player.IsDead)
                return true;
        }

        return false;
    }
}
