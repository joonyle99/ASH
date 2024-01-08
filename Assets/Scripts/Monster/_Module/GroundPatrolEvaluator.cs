using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPatrolEvaluator : MonoBehaviour
{
    [Header("Ground Patrol Evaluator")]
    [Space]

    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Transform _wallCheckPointUp;
    [SerializeField] private Transform _wallCheckPointDown;
    [SerializeField] private float _wallCheckDistance = 1f;

    private MonsterBehavior _monster;

    private void Awake()
    {
        _monster = GetComponent<MonsterBehavior>();
    }

    public bool IsCheckWall()
    {
        RaycastHit2D upRayHit = Physics2D.Raycast(_wallCheckPointUp.position, Vector2.right * _monster.RecentDir, _wallCheckDistance, _layerMask);
        RaycastHit2D downRayHit = Physics2D.Raycast(_wallCheckPointDown.position, Vector2.right * _monster.RecentDir, _wallCheckDistance, _layerMask);

        return upRayHit || downRayHit;
    }

    private void OnDrawGizmosSelected()
    {
        if (!_monster)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(_wallCheckPointUp.position, Vector3.right * _monster.RecentDir * _wallCheckDistance);
        Gizmos.DrawRay(_wallCheckPointDown.position, Vector3.right * _monster.RecentDir * _wallCheckDistance);
    }
}
