using UnityEngine;

public abstract class BossBehavior : MonsterBehavior
{
    [field: Header("Boss Behavior")]
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

    [Space]

    [Tooltip("hurt count x health unit = MaxHp")]
    [SerializeField] protected int finalTargetHurtCount;

    [Space]

    [Header("Attack Count")]
    [Tooltip("Count of attacks for Ultimate Skill")]
    [SerializeField] protected RangeInt attackCountRange;
    [SerializeField] protected int targetAttackCount;
    [SerializeField] protected int currentAttackCount;

    [Space]

    [Header("Hit Count")]
    [Tooltip("Count of hits for Groggy state")]
    [SerializeField] protected RangeInt hitCountRange;
    [SerializeField] protected int targetHitCount;
    [SerializeField] protected int currentHitCount;

    protected override void Start()
    {
        base.Start();

        RandomTargetAttackCount();
        RandomTargetHitCount();
    }

    public abstract void AttackPreProcess();
    public abstract void AttackPostProcess();
    public abstract void GroggyPreProcess();
    public abstract void GroggyPostProcess();

    protected void RandomTargetAttackCount()
    {
        if (attackCountRange.Start > attackCountRange.End)
            Debug.LogError("minTargetCount > maxTargetCount");
        else if (attackCountRange.Start < 0)
            Debug.LogError("minTargetCount < 0");
        else if (attackCountRange.End <= 0)
            Debug.LogError("maxTargetCount <= 0");

        targetAttackCount = Random.Range(attackCountRange.Start, attackCountRange.End);
    }
    protected void RandomTargetHitCount()
    {
        if (hitCountRange.Start > hitCountRange.End)
            Debug.LogError("minTargetCount > maxTargetCount");
        else if (hitCountRange.Start < 0)
            Debug.LogError("minTargetCount < 0");
        else if (hitCountRange.End <= 0)
            Debug.LogError("maxTargetCount <= 0");

        targetHitCount = Random.Range(hitCountRange.Start, hitCountRange.End);
    }
}
