using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCameraSetter : MonoBehaviour
{
    [SerializeField] bool _activateOnStart;
    [SerializeField] float _cameraOffsetY = 0.75f;
    [SerializeField]
    Transform[] _invisibleWalls; 

    BossCameraEvent _event;
    private void Start()
    {
        foreach (var t in _invisibleWalls)
            t.gameObject.SetActive(false);
        if (_activateOnStart)
            Activate();
    }
    public void Activate()
    {
        _event = new BossCameraEvent(transform, _cameraOffsetY);
        SceneEffectManager.Instance.PushSceneEvent(_event);
        foreach (var t in _invisibleWalls)
            t.gameObject.SetActive(true);
    }
    public void Deactivate()
    {
        SceneEffectManager.Instance.RemoveSceneEvent(_event);
        foreach (var t in _invisibleWalls)
            t.gameObject.SetActive(false);
    }
}

class BossCameraEvent : SceneEffectEvent
{
    Transform _transform;
    float _offsetY;
    public BossCameraEvent(Transform transform, float offsetY) 
    {
        _transform = transform;
        _offsetY = offsetY;
    }
    public override void OnEnter() 
    {
        SceneEffectManager.Instance.Camera.FollowOnly(_transform);
        SceneEffectManager.Instance.Camera.OffsetY = _offsetY;
    }
    public override void OnExit()
    {
        //SceneEffectManager.Current.Camera.ResetCameraSettings();
    }
}
