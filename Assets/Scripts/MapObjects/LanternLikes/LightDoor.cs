using UnityEngine;

//������ ����Ǵ� ������ �ؾ��ϴµ� ������ �ƴ� �͵�
public class LightDoor : LanternLike
{
    public override bool IsLightOn { get { return true; } }

    public override void OnBeamConnected(LightBeam beam)
    {
        //��� ����Ǹ� ������
        if (LanternSceneContext.Current.IsAllRelationsConnected())
        {
            Debug.Log("������");
        }
    }

}
