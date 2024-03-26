using System;
using UnityEngine;

public class GroundWallDetector : MonoBehaviour
{
    [Header("Ground Check")]
    [Space]

    [SerializeField] LayerMask _groundLayer;
    [SerializeField] Transform _groundCheckTrans;
    [SerializeField] Transform _groundAboveCheckTrans;
    [SerializeField] float _groundCheckRadius;
    [SerializeField] float _groundAboveCheckLength;

    [Header("Climb Check")]
    [Space]

    [SerializeField] LayerMask _climbLayer;
    [SerializeField] Transform _climbCheckTrans;
    [SerializeField] float _climbCheckLength;

    private PlayerBehaviour _player;

    void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
    }

    void Update()
    {
        // Check Ground
        _player.GroundHit = Physics2D.CircleCast(_groundCheckTrans.position, _groundCheckRadius, Vector2.down, 0f, _groundLayer);

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
        // set color
        Gizmos.color = Color.red;

        // Draw Ground Check
        Gizmos.DrawWireSphere(_groundCheckTrans.position, _groundCheckRadius);

        // Draw Ground Above Check
        Gizmos.DrawLine(_groundAboveCheckTrans.position,
            _groundAboveCheckTrans.position + Vector3.up * _groundAboveCheckLength);

        // Draw Wall Check
        Gizmos.DrawLine(_climbCheckTrans.position, _climbCheckTrans.position + _player.PlayerLookDir3D * _climbCheckLength);
    }
}
