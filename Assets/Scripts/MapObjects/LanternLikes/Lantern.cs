using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Lantern : LanternLike, ILightCaptureListener
{
    [SerializeField] float _lightUpTime = 1.5f;
    [Header("References")]
    [SerializeField] Light2D _spotLight;

    public override bool IsLightOn => _isLightOn;
    bool _isLightOn = false;
    float _currentLightFill = 0f;

    void OnLightTurnedOn()
    {
        LanternSceneContext.Current.RecordActivationTime(this);
    }
    void OnLightTurnedOff()
    {
        LanternSceneContext.Current.DisconnectFromAll(this);
    }

    void TurnLightOn()
    {
        if (_isLightOn)
            return;
        _isLightOn = true;
        _spotLight.gameObject.SetActive(true);
        OnLightTurnedOn();
    }
    void TurnLightOff()
    {
        if (!_isLightOn)
            return;
        _isLightOn = false;
        _spotLight.gameObject.SetActive(false);
        OnLightTurnedOff();
    }

    public void OnLightStay(LightCapturer capturer, LightSource lightSource)
    {
        if (_isLightOn)
            return;
        _currentLightFill += Time.deltaTime;
        if (_currentLightFill > _lightUpTime)
        {
            TurnLightOn();
        }
    }
    public void OnLightExit(LightCapturer capturer, LightSource lightSource)
    {
        if (_isLightOn)
            return;
        _currentLightFill = 0f;
    }
}
