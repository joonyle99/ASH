using UnityEngine;

public class LightBeam : MonoBehaviour
{
    LightBeamLineEffect _beamEffect;

    LanternLike _startLantern;
    LanternLike _endLantern;

    BossBehaviour _endBoss;

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
    public void SetLanternsWithBoss(LanternLike start, BossBehaviour end)
    {
        _startLantern = start;
        _endBoss = end;
        _beamEffect = GetComponent<LightBeamLineEffect>();
        _beamEffect.MarkAsLastConnection();
    }
    private void OnEnable()
    {
        if (_startLantern != null)
        {
            if (_endBoss != null)
            {
                _beamEffect.StartBeamEffect(new Transform[] { _startLantern.LightPoint, _endBoss.CenterOfMass });
            }
            else
            {
                _beamEffect.StartBeamEffect(new Transform[] { _startLantern.LightPoint, _endLantern.LightPoint });
            }
        }
    }
    public bool IsConnectedTo(Lantern lantern)
    {
        return _startLantern == lantern || _endLantern == lantern;
    }
}
