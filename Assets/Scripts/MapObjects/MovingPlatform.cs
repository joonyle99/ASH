using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : ToggleableObject
{
    [SerializeField] float _speed = 1;
    [SerializeField] WaypointPath _path;
    Rigidbody2D _rigidbody;
    float _travelDistance = 0f;
    PreserveState _statePreserver;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
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
    }

    protected override void OnTurnedOn()
    {
    }

    void FixedUpdate()
    {
        if (IsOn)
        {
            _travelDistance += _speed * Time.fixedDeltaTime;
            if (_travelDistance > _path.TotalDistance)
                _travelDistance = _path.TotalDistance;
        }
        else
        {
            _travelDistance -= _speed * Time.fixedDeltaTime;
            if (_travelDistance < 0)
                _travelDistance = 0;
        }
            _rigidbody.MovePosition(_path.GetPosition(_travelDistance));
    }

}
