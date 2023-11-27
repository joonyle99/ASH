using UnityEngine;

//광선이 연결되는 판정은 해야하는데 랜턴은 아닌 것들
public class LightDoor : LanternLike
{
    public override bool IsLightOn { get { return true; } }

    public override void OnBeamConnected(LightBeam beam)
    {
        //모두 연결되면 문열림
        if (LanternSceneContext.Current.IsAllRelationsConnected())
        {
            Debug.Log("문열림");
        }
    }

}
