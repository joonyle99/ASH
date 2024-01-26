using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class FixCameraSceneEffect : MonoBehaviour
{
    [SerializeField] bool _activateOnStart;

    FixCameraEvent _event;
    private void Start()
    {
        if (_activateOnStart)
            Activate();
    }
    public void Activate()
    {
        _event = new FixCameraEvent(transform);
        SceneEffectManager.Current.PushSceneEvent(_event);
    }
    public void Deactivate()
    {
        SceneEffectManager.Current.RemoveSceneEvent(_event);
    }
}

class FixCameraEvent : SceneEffectEvent
{
    Transform _transform;
    public FixCameraEvent(Transform transform) 
    {
        _transform = transform;
    }
    public override void OnEnter() 
    {
        SceneEffectManager.Current.Camera.FollowOnly(_transform);
    }
    public override void OnExit()
    {
        SceneEffectManager.Current.Camera.FollowOnly(SceneContext.Current.Player.transform);
    }
}
