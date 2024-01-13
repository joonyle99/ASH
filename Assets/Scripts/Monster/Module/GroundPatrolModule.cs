using UnityEngine;

public class GroundPatrolModule : MonoBehaviour
{
    [Header("Ground Patrol Module")]
    [Space]

    [SerializeField] private LayerMask _wallCheckLayer;
    [SerializeField] private BoxCollider2D _wallCheckCollider;

    public bool IsWallCheck()
    {
        var rayHit = Physics2D.BoxCast(_wallCheckCollider.transform.position, _wallCheckCollider.bounds.size, 0f, Vector2.zero, 0f,
            _wallCheckLayer);

        return rayHit;
    }
}
