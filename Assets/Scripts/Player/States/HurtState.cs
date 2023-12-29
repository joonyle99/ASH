using System.Collections;
using UnityEngine;

public class HurtState : PlayerState
{
    [SerializeField] private float _godModeTime = 1f;

    protected override void OnEnter()
    {
        Animator.SetTrigger("Hurt");

        SceneContext.Current.Player.IsHurtable = false;

        StartCoroutine(InvincibilityTimer());
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

    private IEnumerator InvincibilityTimer()
    {
        SceneContext.Current.Player.IsGodMode = true;

        yield return new WaitForSeconds(_godModeTime);

        SceneContext.Current.Player.IsGodMode = false;
    }

    public void EndHurt_AnimEvent()
    {
        SceneContext.Current.Player.IsHurtable = true;
        ChangeState<IdleState>();
    }
}