using System.Collections;
using UnityEngine;

public class InstantRespawnState : PlayerState
{
    protected override bool OnEnter()
    {
        StartCoroutine(EnterCoroutine());

        return true;
    }
    protected override bool OnUpdate()
    {

        return true;
    }
    protected override bool OnExit()
    {
        StartCoroutine(ExitCoroutine());

        return true;
    }

    private IEnumerator EnterCoroutine()
    {
        // stay still input setter
        InputManager.Instance.ChangeToStayStillSetter();

        // pause player
        Player.Animator.speed = 0;
        Player.enabled = false;
        Player.Rigidbody.simulated = false;
        Player.Rigidbody.velocity = Vector2.zero;

        // die effect
        Player.SoundList.PlaySFX("SE_Die_02(Short)");
        Player.MaterialController.DisintegrateEffect.Play(0f, false);
        yield return new WaitUntil(() => Player.MaterialController.DisintegrateEffect.IsEffectDone);
        Player.MaterialController.DisintegrateEffect.ResetIsEffectDone();

        // move to check point & change to idle state
        SceneContext.Current.InstantRespawn();
    }
    private IEnumerator ExitCoroutine()
    {
        // respawn effect
        Player.MaterialController.DisintegrateEffect.Play(0f, true);
        yield return new WaitUntil(() => Player.MaterialController.DisintegrateEffect.IsEffectDone);
        Player.MaterialController.DisintegrateEffect.ResetIsEffectDone();

        // resume player
        Player.Rigidbody.velocity = Vector2.zero;
        Player.Rigidbody.simulated = true;
        Player.enabled = true;
        Player.Animator.speed = 1;

        // defualt input setter
        InputManager.Instance.ChangeToDefaultSetter();
    }
}
