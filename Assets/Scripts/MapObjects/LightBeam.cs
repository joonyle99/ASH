using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeam : MonoBehaviour
{
    LightBeamLineEffect _beamEffect;

    LanternLike _startLantern;
    LanternLike _endLantern;

    public bool IsShootingDone { get { return _beamEffect.IsShootingDone; } }
    public Vector3 CurrentShootingPosition { get { return _beamEffect.CurrentShootingPosition; } }
    private void Awake()
    {
        _beamEffect = GetComponent<LightBeamLineEffect>();
    }
    public void SetLanterns(LanternLike start, LanternLike end)
    {
        _startLantern = start;
        _endLantern = end;
        _beamEffect = GetComponent<LightBeamLineEffect>();
        if (_endLantern == LanternSceneContext.Current.LightDoor)
            _beamEffect.MarkAsLastConnection();
    }
    private void OnEnable()
    {
        if (_startLantern != null)
            _beamEffect.StartBeamEffect(new Transform[] { _startLantern.transform, _endLantern.transform });
    }
    public bool IsConnectedTo(Lantern lantern)
    {
        return _startLantern == lantern || _endLantern == lantern;
    }
}
