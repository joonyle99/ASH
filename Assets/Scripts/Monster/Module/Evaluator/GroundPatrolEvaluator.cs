using UnityEngine;

public class GroundPatrolEvaluator : MonoBehaviour
{
    [Header("Ground Patrol Evaluator")]
    [Space]

    [SerializeField] private LayerMask _wallCheckLayer;
    [SerializeField] private Transform _wallCheckTrans;
    [SerializeField] private Vector2 _WallCheckBoxSize;

    public bool IsWallCheck()
    {
        var rayHit = Physics2D.BoxCast(_wallCheckTrans.position, _WallCheckBoxSize, 0f, Vector2.zero, 0f,
            _wallCheckLayer);

        return rayHit;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_wallCheckTrans.position, new Vector3(_WallCheckBoxSize.x, _WallCheckBoxSize.y, 0f));
    }
}
