using System.Collections;
using UnityEngine;

public class M_HurtState : MonsterState
{
    protected override void OnEnter()
    {
        Animator.SetTrigger("Hurt");
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnFixedUpdate()
    {

    }

    protected override void OnExit()
    {

    }

    private IEnumerator TwinkleEffect()
    {
        // TODO
        yield return new WaitForSeconds(1f);
    }

    public void EndHurt_AnimEvent()
    {
        ChangeState<M_IdleState>();
    }
}