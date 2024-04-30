using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BlackPanther : BossBehavior, ILightCaptureListener
{
    #region Variable

    [Header("BlackPanther")]
    [Space]
    
    private int temp;

    #endregion

    #region Function

    protected override void Start()
    {
        base.Start();

        monsterData.MaxHp = finalTargetHurtCount * MonsterDefine.BossHealthUnit;
        CurHp = monsterData.MaxHp;
        IsGodMode = true;
    }
    public void FixedUpdate()
    {
        if (IsDead)
            return;

        if (CurrentStateIs<GroundMoveState>())
        {
            if (GroundMovementModule)
                GroundMovementModule.GroundWalking();
        }
    }

    public override IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {

        return IAttackListener.AttackResult.Success;
    }
    public override void Die(bool isHitBoxDisable = true, bool isDeathProcess = true)
    {

    }

    public void OnLightEnter(LightCapturer capturer, LightSource lightSource)
    {

    }

    public override void AttackPreProcess()
    {

    }
    public override void AttackPostProcess()
    {

    }
    public override void GroggyPreProcess()
    {

    }
    public override void GroggyPostProcess()
    {

    }

    #endregion

    private void OnDrawGizmosSelected()
    {

    }
}
