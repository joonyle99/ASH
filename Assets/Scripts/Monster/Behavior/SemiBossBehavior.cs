using UnityEngine;

public abstract class SemiBossBehavior : MonsterBehavior
{
    [field: Header("SemiBoss Behavior")]
    [field: Space]

    [field: SerializeField]
    public bool IsGroggy
    {
        get;
        set;
    }
    [field: SerializeField]
    public bool IsRage
    {
        get;
        set;
    }

    public abstract void AttackPreProcess();
    public abstract void AttackPostProcess();

    public abstract void GroggyPreProcess();
    public abstract void GroggyPostProcess();
}
