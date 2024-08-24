public sealed class Turtle : MonsterBehaviour, ISceneContextBuildListener
{
    private PreserveState _statePreserver;

    protected override void Awake()
    {
        base.Awake();

        _statePreserver = GetComponent<PreserveState>();
    }
    public void OnSceneContextBuilt()
    {
        if (_statePreserver)
        {
            bool isDead = _statePreserver.LoadState("_isDeadSaved", IsDead);

            if (isDead)
            {
                Die(false, false);
            }

            if (SceneChangeManager.Instance.SceneChangeType == SceneChangeType.Loading)
            {
                RecentDir = _statePreserver.LoadState<int>("_recentDirSaved", DefaultDir);
            }
            else
            {
                RecentDir = _statePreserver.LoadState<int>("_recentDir", DefaultDir);
            }

            SetRecentDir(RecentDir);
        }

        SaveAndLoader.OnSaveStarted += SaveTurtleState;
    }
    
    // destroy function
    private void OnDestroy()
    {
        if (_statePreserver)
        {
            _statePreserver.SaveState<int>("_recentDir", RecentDir);
        }

        SaveAndLoader.OnSaveStarted -= SaveTurtleState;
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

    public override IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        // ** �ź��̴� Dead ���¿����� Hit�� �����ϴ� **

        // ���ų� ��� ���¿��� �ǰ� ó��
        if (IsHiding || IsDead)
            HitProcess(attackInfo, false, true, false);
        // �Ϲ����� ������ �ǰ� ó��
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

    private void SaveTurtleState()
    {
        if (_statePreserver)
        {
            _statePreserver.SaveState("_isDeadSaved", IsDead);
            _statePreserver.SaveState("_recentDirSaved", RecentDir);
        }
    }
}
