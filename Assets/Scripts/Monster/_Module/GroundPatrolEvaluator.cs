using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPatrolEvaluator : MonoBehaviour
{
    [Header("Ground Patrol Evaluator")]
    [Space]

    [SerializeField] private MonsterBehavior _monster;

    [Space]

    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Transform _wallCheckPointUp;
    [SerializeField] private Transform _wallCheckPointDown;
    [SerializeField] private float _wallCheckDistance = 1f;

    void Update()
    {
        RaycastHit2D upRayHit = Physics2D.Raycast(_wallCheckPointUp.position, Vector2.right * _monster.RecentDir, _wallCheckDistance, _layerMask);
        RaycastHit2D downRayHit = Physics2D.Raycast(_wallCheckPointDown.position, Vector2.right * _monster.RecentDir, _wallCheckDistance, _layerMask);
        if (upRayHit || downRayHit)
            _monster.UpdateImageFlip();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_wallCheckPointUp.position, Vector3.right * _monster.RecentDir * _wallCheckDistance);
        Gizmos.DrawRay(_wallCheckPointDown.position, Vector3.right * _monster.RecentDir * _wallCheckDistance);
    }
}
