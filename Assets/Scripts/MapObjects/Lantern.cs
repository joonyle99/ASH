using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Lantern : MonoBehaviour, ILightCaptureListener
{
    public bool IsLightOn => _isLightOn;
    [SerializeField] float _lightUpTime = 1.5f;
    [Header("References")]
    [SerializeField] Light2D _spotLight;

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

    public void TurnLightOn()
    {
        if (_isLightOn)
            return;
        _isLightOn = true;
        _spotLight.gameObject.SetActive(true);
        OnLightTurnedOn();
    }
    public void TurnLightOff()
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


#if X
[CustomEditor(typeof(Lantern))]
public class LanternInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var t = target as Lantern;
        if (GUILayout.Button("On"))
        {
            t.TurnLightOn();
        }
        if (GUILayout.Button("Off"))
        {
            t.TurnLightOff();
        }

    }
}
#endif