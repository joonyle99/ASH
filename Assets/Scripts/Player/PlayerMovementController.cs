using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] float _maxSpeed = 8f;
    [SerializeField] float _acceleration = 15f;
    [SerializeField] float _decceleration = 15f;

    PlayerBehaviour _player;

    Vector2 _moveForce;

    void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
    }


    void FixedUpdate()
    {
        if (!_player.IsGrounded)
            return;

        Vector2 groundNormal = _player.GroundHit.normal;
        Vector2 moveDir;
        if (_player.PlayerLookDir2D.x > 0f)
            moveDir = (-1) * Vector2.Perpendicular(groundNormal);
        else
            moveDir = Vector2.Perpendicular(groundNormal);

        Vector2 targetVelocity = moveDir * _maxSpeed;
        Vector2 velocityNeeded = targetVelocity - _player.Rigidbody.velocity;
        float accelRate = (velocityNeeded.magnitude > 0.01f) ? _acceleration : _decceleration;

        _moveForce = velocityNeeded * accelRate;

        _player.Rigidbody.AddForce(_moveForce);
    }
    private void OnDrawGizmosSelected()
    {
        // 플레이어가 이동하는 방향
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(_moveForce.x, _moveForce.y, 0));
    }
}
