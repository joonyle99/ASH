using UnityEngine;

public class BossCameraSetter : MonoBehaviour
{
    [SerializeField] bool _activateOnStart;
    [SerializeField] float _cameraOffsetY = 0.75f;

    BossCameraEvent _event;

    private void Start()
    {
        if (_activateOnStart)
            Activate();
    }

    public void Activate()
    {
        _event = new BossCameraEvent(transform, _cameraOffsetY);
        SceneEffectManager.Instance.PushSceneEvent(_event);
    }
    public void Deactivate()
    {
        SceneEffectManager.Instance.RemoveSceneEvent(_event);
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
        Debug.Log($"SceneEffectManager.Instance.Camera.OffsetY: {SceneEffectManager.Instance.Camera.OffsetY}\n" +
                  $"_offsetY: {_offsetY}");

        SceneEffectManager.Instance.Camera.StartFollow(_transform);
        SceneEffectManager.Instance.Camera.OffsetY = _offsetY;
    }
    public override void OnExit()
    {
        //SceneEffectManager.Current.Camera.ResetCameraSettings();
    }
}
