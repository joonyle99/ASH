using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenPath : MonoBehaviour, ILightCaptureListener
{
    [SerializeField] HiddenPathMask _mask;
    [SerializeField] HiddenPathMask.Direction _swipeDirection;
    [SerializeField] float _swipeDuration;
    [SerializeField] GameObject _destroyingCollidersParent;

    LayerMask [] _originalColliderLayers;
    void Awake()
    {
        _mask.InitMask(_swipeDirection);
    }
    public void OnLightStay(LightCapturer capturer, LightSource lightSource)
    {
        _mask.OnLightCaptured(_swipeDuration);
        Destroy(capturer.gameObject);
        Destroy(_destroyingCollidersParent);
    }

}
