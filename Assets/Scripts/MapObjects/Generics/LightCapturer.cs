using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCapturer : MonoBehaviour
{
    [RequireInterface(typeof(ILightCaptureListener))][SerializeField] Object _lightCaptureListenerObject;
    ILightCaptureListener _lightCaptureListener => _lightCaptureListenerObject as ILightCaptureListener;

    bool _isGettingLighted = false;
    bool _wasGettingLightedLastFrame = false;
    LightSource _lastLightSource = null;
    public bool IsGettingLighted { get { return _isGettingLighted; } }

    private void LateUpdate()
    {
        if (!_isGettingLighted && _wasGettingLightedLastFrame)
        {
            _isGettingLighted = false;

            if (_lightCaptureListener != null)
                _lightCaptureListener.OnLightExit(this, _lastLightSource);
            _lastLightSource = null;
        }
        _wasGettingLightedLastFrame = _isGettingLighted;
        _isGettingLighted = false;
    }
    public void OnLightHitted(LightSource lightSource)
    {
        _isGettingLighted = true;

        if (_lightCaptureListener != null)
        {
            if (!_wasGettingLightedLastFrame)
                _lightCaptureListener.OnLightEnter(this, lightSource);
            else
                _lightCaptureListener.OnLightStay(this, lightSource);
        }
        _lastLightSource = lightSource;
    }
}
