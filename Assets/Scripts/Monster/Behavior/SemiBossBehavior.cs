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

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
    }
    public override void SetUp()
    {
        base.SetUp();
    }

    public override void KnockBack(Vector2 forceVector)
    {
        base.KnockBack(forceVector);
    }
    public override IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        return base.OnHit(attackInfo);
    }
    public override void Die()
    {
        base.Die();
    }

    public abstract void AttackPreProcess();
    public abstract void AttackPostProcess();

    public abstract void GroggyPreProcess();
    public abstract void GroggyPostProcess();
}
