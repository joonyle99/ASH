using System;
using UnityEngine;

[ExecuteInEditMode]
public class GroundWallDetector : MonoBehaviour
{
    [Header("Ground Check")]
    [Space]

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _groundCheckTrans;
    [SerializeField] private Transform _groundUpwardCheckTrans;
    [SerializeField] private Transform _groundBackwardCheckTrans;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private float _groundUpwardCheckRadius;
    [SerializeField] private float _groundUpwardCheckDistance;
    [SerializeField] private float _groundBackwardCheckDistance;

    [Header("Climb Check")]
    [Space]

    [SerializeField] private LayerMask _climbLayer;
    [SerializeField] private Transform _climbCheckTrans;
    [SerializeField] private float _climbCheckLength;

    private PlayerBehaviour _player;

    void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
    }

    void Update()
    {
        // Check Ground
        _player.GroundHit = Physics2D.CircleCast(_groundCheckTrans.position, _groundCheckRadius, Vector2.down, 0f, _groundLayer);

        // Check Upward Ground
        _player.UpwardGroundHit = Physics2D.CircleCast(_groundUpwardCheckTrans.position, _groundUpwardCheckRadius, Vector2.up, 0f, _groundLayer);

        // Check Upward Ground (For Wall Climb)
        _player.UpwardGroundHitForClimb = Physics2D.Raycast(_groundUpwardCheckTrans.position, Vector2.up, _groundUpwardCheckDistance, _groundLayer);

        // Check Backward Wall
        _player.BackwardGroundHit = Physics2D.Raycast(_groundBackwardCheckTrans.position, -_player.PlayerLookDir2D, _groundBackwardCheckDistance, _groundLayer);
        
        // Check Wall
        _player.ClimbHit = Physics2D.Raycast(_climbCheckTrans.position, _player.PlayerLookDir2D, _climbCheckLength, _climbLayer);

        if (_player.ClimbHit)
        {
            // TODO : 벽의 방향을 localScale로 하면 위험하다
            var wallLookDir = Math.Sign(_player.ClimbHit.transform.localScale.x);
            var isDirSync = (wallLookDir * _player.RecentDir) > 0;
            _player.IsClimbable = !isDirSync;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_player == null)
            return;

        Gizmos.color = Color.red;

        // Draw Ground Check
        Gizmos.DrawWireSphere(_groundCheckTrans.position, _groundCheckRadius);

        // Draw Upward Ground Check
        Gizmos.DrawWireSphere(_groundUpwardCheckTrans.position, _groundUpwardCheckRadius);

        // Draw Wall Check
        Gizmos.DrawRay(_climbCheckTrans.position, _player.PlayerLookDir3D * _climbCheckLength);

        // Draw Wall Check
        Gizmos.DrawRay(_groundBackwardCheckTrans.position, _player.PlayerLookDir2D * _groundBackwardCheckDistance);
    }
}
