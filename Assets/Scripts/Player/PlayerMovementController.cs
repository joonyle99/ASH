using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float _maxSpeed = 8f;
    [SerializeField] private float _acceleration = 37.5f;
    [SerializeField] private float _deceleration = 37.5f;

    private HashSet<object> _disableMovementList;

    private PlayerBehaviour _player;
    private Vector2 _moveForce;

    private void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();

        _disableMovementList = new HashSet<object>();
    }
    private void FixedUpdate()
    {
        if (!_player.IsGrounded || !_player.IsMoveXKey || _disableMovementList.Count > 0)
            return;

        // 지면의 기울기에 따른 이동 방향 설정
        Vector2 groundNormal = _player.GroundHit.normal;
        Vector2 moveDirection = _player.RawInputs.Movement.x > 0f
            ? (-1) * Vector2.Perpendicular(groundNormal)
            : Vector2.Perpendicular(groundNormal);

        // 목표까지의 필요한 속도 계산
        Vector2 targetVelocity = moveDirection * _maxSpeed;
        Vector2 velocityNeeded = targetVelocity - Vector2.Dot(_player.Rigidbody.velocity, moveDirection) * moveDirection;

        // 최종적으로 가해줄 힘을 계산
        float accelRate = (velocityNeeded.magnitude > 0.01f) ? _acceleration : _deceleration;
        Vector2 moveForce = velocityNeeded * accelRate;

        // for gizmos
        _moveForce = moveForce;

        // 플레이어에게 힘을 가함
        _player.Rigidbody.AddForce(moveForce);
    }

    public void DisableMovementExternal(object owner)
    {
        _disableMovementList.Add(owner);
    }
    public void EnableMovementExternal(object owner)
    {
        _disableMovementList.Remove(owner);
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
