using UnityEngine;

//������ ����Ǵ� ������ �ؾ��ϴµ� ������ �ƴ� �͵�
public class LightDoor : LanternLike
{
    private void Start()
    {
        IsLightOn = true;
    }

    public override void OnBeamConnected(LightBeam beam)
    {
        //��� ����Ǹ� ������
        if (LanternSceneContext.Current.IsAllRelationsConnected())
        {
            Debug.Log("������");
        }
    }

}
