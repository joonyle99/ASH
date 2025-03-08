using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    [SerializeField] private Vector3 _moveDir;
    [SerializeField] private float _speed;

    public void SetData(Vector3 newMoveDir, float newSpeed)
    {
        _moveDir = newMoveDir;
        _speed = newSpeed * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        transform.position += _moveDir * _speed;
    }
}