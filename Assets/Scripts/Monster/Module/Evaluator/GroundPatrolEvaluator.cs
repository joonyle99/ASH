using UnityEngine;

public class GroundPatrolEvaluator : MonoBehaviour
{
    [Header("Ground Patrol Evaluator")]
    [Space]

    [SerializeField] private LayerMask _wallCheckLayer;
    [SerializeField] private BoxCollider2D _wallCheckCollider;
    private Vector2 _WallCheckBoxSize;

    private void Awake()
    {
        _WallCheckBoxSize = _wallCheckCollider.bounds.size;
    }

    public bool IsWallCheck()
    {
        var rayHit = Physics2D.BoxCast(_wallCheckCollider.transform.position, _WallCheckBoxSize, 0f, Vector2.zero, 0f,
            _wallCheckLayer);

        return rayHit;
    }
}
