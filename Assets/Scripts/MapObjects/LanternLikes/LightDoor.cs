using UnityEngine;

//광선이 연결되는 판정은 해야하는데 랜턴은 아닌 것들
public class LightDoor : LanternLike
{
    private void Start()
    {
    }
    public void Update()
    {
        if (LanternSceneContext.Current.IsAllRelationsFullyConnected(this))
        {
            IsLightOn = true;
        }
    }
    public override void OnBeamConnected(LightBeam beam)
    {
            Debug.Log("문열림");
    }

}
