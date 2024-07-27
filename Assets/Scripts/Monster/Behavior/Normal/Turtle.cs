public sealed class Turtle : MonsterBehaviour
{
    private PreserveState _statePreserver;

    protected override void Awake()
    {
        base.Awake();

        _statePreserver = GetComponent<PreserveState>();

        SaveAndLoader.OnSaveStarted += SaveDeadState;
    }
    protected override void Start()
    {
        base.Start();

        if (_statePreserver)
        {
            bool isDead = _statePreserver.LoadState("_isDead", IsDead);
            if (isDead)
            {
                Die(false, false);
            }
        }
    }
    public void FixedUpdate()
    {
        if (IsDead)
            return;

        if (CurrentStateIs<GroundMoveState>())
        {
            if (GroundMovementModule)
                GroundMovementModule.WalkGround();
        }
        else if (CurrentStateIs<Monster_IdleState>())
        {
            if (GroundMovementModule)
                GroundMovementModule.AffectGravity();
        }
    }

    // destroy function
    private void OnDestroy()
    {
        
    }

    public override IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        // ** 거북이는 Dead 상태에서도 Hit가 가능하다 **

        // 숨거나 사망 상태에서 피격 처리
        if (IsHiding || IsDead)
            HitProcess(attackInfo, false, true, false);
        // 일반적인 상태의 피격 처리
        else
            HitProcess(attackInfo, false, true, true);

        // Change to Hurt State
        if (CurrentState is IHurtableState)
            SetAnimatorTrigger("Hurt");

        return IAttackListener.AttackResult.Success;
    }
    public override void Die(bool isHitBoxDisable, bool isDeathProcess)
    {
        base.Die(false, false);

        // Trigger -> Collision
        SetHitBoxStepable(true);
    }

    private void SaveDeadState()
    {
        if (_statePreserver)
        {
            _statePreserver.SaveState("_isDead", IsDead);
        }
    }
}
