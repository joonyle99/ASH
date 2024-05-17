using System;
using UnityEngine;

[ExecuteInEditMode]
public class GroundWallDetector : MonoBehaviour
{
    [Header("Ground Check")]
    [Space]

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _groundCheckTrans;
    [SerializeField] private Transform _groundAboveCheckTrans;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private float _groundCheckDistance;
    [SerializeField] private float _groundAboveCheckLength;

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
        var circleCastTarget = Physics2D.CircleCast(_groundCheckTrans.position, _groundCheckRadius, Vector2.down, 0f,
            _groundLayer);

        if (circleCastTarget)
            _player.GroundHit = circleCastTarget;
        else
        {
            var rayCastTarget = Physics2D.Raycast(_groundCheckTrans.position, Vector2.down, _groundCheckDistance,
                _groundLayer);

            _player.GroundHit = rayCastTarget;
        }

        // Check Upward
        _player.UpwardGroundHit = Physics2D.Raycast(transform.position, Vector2.up, _groundAboveCheckLength, _groundLayer);

        // Check Climb
        _player.ClimbHit = Physics2D.Raycast(_climbCheckTrans.position, _player.PlayerLookDir2D, _climbCheckLength, _climbLayer);
        if (_player.ClimbHit)
        {
            // TODO : 벽의 방향을 localScale로 하면 위험하다
            int wallLookDir = Math.Sign(_player.ClimbHit.transform.localScale.x);
            bool isDirSync = (wallLookDir * _player.RecentDir) > 0;
            _player.IsClimbable = !isDirSync;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(_player == null)
            return;

        // set color
        Gizmos.color = Color.red;

        // Draw Ground Check
        Gizmos.DrawWireSphere(_groundCheckTrans.position, _groundCheckRadius);
        Gizmos.DrawRay(_groundCheckTrans.position, Vector3.down * _groundCheckDistance);

        // Draw Ground Above Check
        Gizmos.DrawRay(_groundAboveCheckTrans.position, Vector3.up * _groundAboveCheckLength);

        // Draw Wall Check
        Gizmos.DrawRay(_climbCheckTrans.position, _player.PlayerLookDir3D * _climbCheckLength);
    }
}
