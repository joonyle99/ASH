using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenPath : MonoBehaviour, ILightCaptureListener, ITriggerListener
{
    [SerializeField] HiddenPathMask _mask;
    [SerializeField] HiddenPathMask.Direction _swipeDirection;
    [SerializeField] float _swipeDuration;
    [SerializeField] Collider2D [] _disablingColliders;

    LayerMask [] _originalColliderLayers;
    void Awake()
    {
        _mask.InitMask(_swipeDirection);
    }
    public void OnLightCaptured(LightCapturer capturer, LightSource lightSource)
    {
        _mask.OnLightCaptured(_swipeDuration);
        Destroy(capturer.gameObject);
    }

    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter)
    { 
        if (activator.Type == ActivatorType.Player)
        {
            _originalColliderLayers = new LayerMask[_disablingColliders.Length];
            for (int i=0; i< _disablingColliders.Length; i++)
            {
                _originalColliderLayers[i] = _disablingColliders[i].gameObject.layer;
                _disablingColliders[i].gameObject.layer = LayerMask.NameToLayer("ExceptPlayer");
            }
        }
    }
    public void OnExitReported(TriggerActivator activator, TriggerReporter reporter)
    {
        if (activator.Type == ActivatorType.Player)
        {
            for (int i = 0; i < _disablingColliders.Length; i++)
            {
                _disablingColliders[i].gameObject.layer = _originalColliderLayers[i];
            }
        }
    }
}
