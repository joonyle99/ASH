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
        if (!_player.IsGrounded || _player.RawInputs.Movement.x == 0)
            return;

        Vector2 groundNormal = _player.GroundHit.normal;
        Vector2 moveDir;
        if (_player.RawInputs.Movement.x > 0f)
            moveDir = (-1) * Vector2.Perpendicular(groundNormal);
        else
            moveDir = Vector2.Perpendicular(groundNormal);

        Vector2 targetVelocity = moveDir * _maxSpeed;
        Vector2 velocityNeeded = targetVelocity - Vector2.Dot(_player.Rigidbody.velocity, moveDir) * moveDir;
        float accelRate = (velocityNeeded.magnitude > 0.01f) ? _acceleration : _decceleration;

        _moveForce = velocityNeeded * accelRate;
        _player.Rigidbody.AddForce(_moveForce);
    }
    private void OnDrawGizmosSelected()
    {
        // 플레이어가 이동하는 방향
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(_moveForce.x, _moveForce.y, 0));

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(_player.Rigidbody.velocity.x, _player.Rigidbody.velocity.y, 0));

        }
    }
}
