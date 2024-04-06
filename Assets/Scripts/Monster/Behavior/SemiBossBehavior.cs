using UnityEngine;

public abstract class SemiBossBehavior : MonsterBehavior
{
    [Header("SemiBossBehavior")]
    [Space]

    [Header("Condition")]
    [Space]

    [SerializeField] private bool _isGroggy;
    public bool IsGroggy
    {
        get => _isGroggy;
        set => _isGroggy = value;
    }
    [SerializeField] private bool _isRage;
    public bool IsRage
    {
        get => _isRage;
        set => _isRage = value;
    }

    public abstract void AttackPreProcess();
    public abstract void AttackPostProcess();

    public abstract void GroggyPreProcess();
    public abstract void GroggyPostProcess();
}
