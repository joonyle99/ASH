using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//������ ����Ǵ� ������ �ؾ��ϴµ� ������ �ƴ� �͵�
public abstract class LanternLike : MonoBehaviour
{
    public abstract bool IsLightOn { get; }

    public virtual void OnBeamConnected(LightBeam beam) { }
    public virtual void OnBeamDisconnected(LightBeam beam){ }
}
