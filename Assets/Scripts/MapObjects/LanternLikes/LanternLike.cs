using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//광선이 연결되는 판정은 해야하는데 랜턴은 아닌 것들
public abstract class LanternLike : MonoBehaviour
{
    public abstract bool IsLightOn { get; }

    public virtual void OnBeamConnected(LightBeam beam) { }
    public virtual void OnBeamDisconnected(LightBeam beam){ }
}
