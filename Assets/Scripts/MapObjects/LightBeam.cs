using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeam : MonoBehaviour
{
    LightBeamLineEffect _beamEffect;

    LanternLike _startLantern;
    LanternLike _endLantern;

    public bool IsShootingDone { get { return _beamEffect.IsShootingDone; } }
    private void Awake()
    {
        _beamEffect = GetComponent<LightBeamLineEffect>();
    }
    public void SetLanterns(LanternLike start, LanternLike end)
    {
        _startLantern = start;
        _endLantern = end;
    }
    void ResetLinePositions()
    {
        if (_startLantern == null)
            return;
        _beamEffect.StartBeamEffect(new Transform[]{ _startLantern.transform, _endLantern.transform});
    }
    void Start()
    {
        ResetLinePositions();
        //TODO : Connecting animation
    }
    private void OnEnable()
    {
        ResetLinePositions();
    }
    public bool IsConnectedTo(Lantern lantern)
    {
        return _startLantern == lantern || _endLantern == lantern;
    }
}
