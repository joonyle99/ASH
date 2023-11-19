using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Lantern : MonoBehaviour
{
    public bool IsLightOn => _isLightOn;

    [Header("References")]
    [SerializeField] Light2D _spotLight;

    bool _isLightOn = false;

    

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



}


#if UNITY_EDITOR
[CustomEditor(typeof(Lantern))]
public class LanternInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var t = target as Lantern;
        if (GUILayout.Button("On"))
        {
            t.TurnLightOn();
        }
        if (GUILayout.Button("Off"))
        {
            t.TurnLightOff();
        }
        base.OnInspectorGUI();

    }
}
#endif