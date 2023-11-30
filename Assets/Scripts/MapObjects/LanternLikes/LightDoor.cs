using UnityEngine;

//������ ����Ǵ� ������ �ؾ��ϴµ� ������ �ƴ� �͵�
public class LightDoor : LanternLike
{
    public enum State
    {
        Closed, Opening, Opened, Closing
    }
    public State CurrentState { get; private set; } = State.Closed;
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
        CurrentState = State.Opening;
        Invoke("OnOpen", 1);
    }
    void OnOpen()
    {
        CurrentState = State.Opened;
    }
}
