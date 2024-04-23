using UnityEngine;

public enum ObjectType
{
    None, Ground, FallingTree, RollingStoneBig, RollingStoneSmall, SquareStone, Spikes, FallingSpike, Player, WaterPool
}

public class ASHObject : MonoBehaviour
{
    [SerializeField] ObjectType _objectType;

    public ObjectType Type => _objectType;
}