using UnityEngine;
using UnityEngine.Animations.Rigging;

public sealed class Turtle : MonsterBehaviour, ISceneContextBuildListener
{
    private PreserveState _statePreserver;

    protected override void Awake()
    {
        base.Awake();

        _statePreserver = GetComponent<PreserveState>();

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
                //Debug.Log("Change Scene by Loading, Recent direction value : " + RecentDir);
            }
            else
            {
                int tmpDir = DefaultDir;
                tmpDir = _statePreserver.LoadState<int>("_recentDir", DefaultDir);
                RecentDir = tmpDir;
                //Debug.Log("Change Scene by Loading, Recent direction value : " + tmpDir);
                //SetRecentDir(tmpDir);
            }

            Debug.Log($"Call awake in Turtle");
            AlignModelToMovement();
        }

        SaveAndLoader.OnSaveStarted += SaveTurtleState;
    }

    public void OnSceneContextBuilt()
    {
    }
    
    // destroy function
    private void OnDestroy()
    {
        if (_statePreserver)
        {
            if(SceneChangeManager.Instance != null &&
                SceneChangeManager.Instance.SceneChangeType != SceneChangeType.StageReset)
            {
                _statePreserver.SaveState<int>("_recentDir", RecentDir);
            }
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

    private void SaveTurtleState()
    {
        if (_statePreserver)
        {
            _statePreserver.SaveState("_isDeadSaved", IsDead);
            _statePreserver.SaveState("_recentDirSaved", RecentDir);
        }
    }

    /// <summary>
    /// RecentDir 값 변경 후 호출 (필수 x)
    /// </summary>
    private void AlignModelToMovement()
    {
        int sign = RecentDir > 0 ? -1 : 1;
        transform.localScale = new Vector3(sign * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}
