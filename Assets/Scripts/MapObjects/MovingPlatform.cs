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


    SceneEffectEvent _recentEvent;
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
        if (!_isMoving)
            _recentEvent = SceneContext.Current.SceneEffectManager.PushSceneEvent(new SceneEvents.FollowObjects(SceneEffectEvent.EventPriority.MovingObjects,
                                                        SceneEffectEvent.MergePolicy.PlayTogether, transform));
        _isMoving = true;
    }

    protected override void OnTurnedOn()
    {
        if (!_isMoving)
            _recentEvent = SceneContext.Current.SceneEffectManager.PushSceneEvent(new SceneEvents.FollowObjects(SceneEffectEvent.EventPriority.MovingObjects,
                                                        SceneEffectEvent.MergePolicy.PlayTogether, transform));
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
        SceneContext.Current.SceneEffectManager.RemoveSceneEvent(_recentEvent);
    }

}
