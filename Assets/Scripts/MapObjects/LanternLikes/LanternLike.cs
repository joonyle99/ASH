using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//������ ����Ǵ� ������ �ؾ��ϴµ� ������ �ƴ� �͵�
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
