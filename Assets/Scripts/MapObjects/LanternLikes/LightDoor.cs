using UnityEngine;

//������ ����Ǵ� ������ �ؾ��ϴµ� ������ �ƴ� �͵�
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
            Debug.Log("������");
    }

}
