using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//광선이 연결되는 판정은 해야하는데 랜턴은 아닌 것들
public abstract class LanternLike : MonoBehaviour
{
    public virtual Transform LightPoint { get { return transform; } }
    public bool IsLightOn
    {
        get { return _isLightOn; }
        set 
        {
            if (value == _isLightOn)
                return;
            _isLightOn = value;
            if(LanternSceneContext.Current != null)
            {
                if (value)
                    LanternSceneContext.Current.RecordActivationTime(this);
                else
                    LanternSceneContext.Current.DisconnectFromAll(this);
            }
        }
    }

    bool _isLightOn = false;
    public virtual void OnBeamConnected(LightBeam beam) { }
    public virtual void OnBeamDisconnected(LightBeam beam){ }

}
