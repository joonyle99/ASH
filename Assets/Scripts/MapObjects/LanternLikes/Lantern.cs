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

    float _currentLightFill = 0f;

    void TurnLightOn()
    {
        if (IsLightOn)
            return;
        IsLightOn = true;
        _spotLight.gameObject.SetActive(true);
    }
    void TurnLightOff()
    {
        if (!IsLightOn)
            return;
        IsLightOn = false;
        _spotLight.gameObject.SetActive(false);
    }

    public void OnLightStay(LightCapturer capturer, LightSource lightSource)
    {
        if (IsLightOn)
            return;
        _currentLightFill += Time.deltaTime;
        if (_currentLightFill > _lightUpTime)
        {
            TurnLightOn();
        }
    }
    public void OnLightExit(LightCapturer capturer, LightSource lightSource)
    {
        if (IsLightOn)
            return;
        _currentLightFill = 0f;
    }
}
