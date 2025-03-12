using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 광선이 연결되는 판정은 해야하는데 랜턴은 아닌 것들
/// </summary>
public abstract class LanternLike : MonoBehaviour
{
    [SerializeField] private Transform _cameraPoint;
    public virtual Transform LightPoint
    {
        get
        {
            if (_cameraPoint == null)
            {
                return this.transform;
            }
            else
            {
                return _cameraPoint;
            }
        }
    }
    public bool IsLightOn
    {
        get { return _isLightOn; }
        set
        {
            if (value == _isLightOn)
                return;
            _isLightOn = value;
            if (LanternSceneContext.Current != null)
            {
                if (value)
                    LanternSceneContext.Current.RecordActivationTime(this);
                else
                    LanternSceneContext.Current.DisconnectFromAll(this);
            }
        }
    }

    bool _isLightOn = false;

    protected virtual void Awake() { }
    protected virtual void Update() { }
    protected virtual void OnDestroy() { }
    protected virtual void FixedUpdate() { }

    public virtual void OnBeamConnected(LightBeam beam) { }
    public virtual void OnBeamDisconnected(LightBeam beam) { }
}
