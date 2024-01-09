using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : ToggleableObject
{
    [SerializeField] float _speed = 1;
    [SerializeField] WaypointPath _path;
    Rigidbody2D _rigidbody;
    float _travelDistance = 0f;
    bool _isMoving = false;
    PreserveState _statePreserver;

    CameraController _cameraController;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _cameraController = Camera.main.GetComponent<CameraController>();
        _statePreserver = GetComponent<PreserveState>();
        if (_statePreserver)
            _travelDistance = _statePreserver.Load("travelDistance", 0f);
    }
    private void OnDestroy()
    {
        if (_statePreserver)
            _statePreserver.Save("travelDistance", _travelDistance);
    }
    protected override void OnTurnedOff()
    {
        if (!_isMoving)
            _cameraController.StartFollow(transform, false);
        _isMoving = true;
    }

    protected override void OnTurnedOn()
    {
        if (!_isMoving)
            _cameraController.StartFollow(transform, false);
        _isMoving = true;
    }

    void FixedUpdate()
    {
        if (_isMoving)
        {
            if (IsOn)
            {
                _travelDistance += _speed * Time.fixedDeltaTime;
                if (_travelDistance > _path.TotalDistance)
                {
                    _travelDistance = _path.TotalDistance;
                    OnStop();
                }
            }
            else
            {
                _travelDistance -= _speed * Time.fixedDeltaTime;
                if (_travelDistance < 0)
                {
                    _travelDistance = 0;
                    OnStop();
                }
            }
        }
        _rigidbody.MovePosition(_path.GetPosition(_travelDistance));
    }
    void OnStop()
    {
        _isMoving = false;
        _cameraController.RemoveFollowTarget(transform);
    }

}
