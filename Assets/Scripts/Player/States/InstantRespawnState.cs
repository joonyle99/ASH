using System.Collections;
using UnityEngine;

public class InstantRespawnState : PlayerState
{
    private PlayerBehaviour _player;

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
        InputManager.Instance.ChangeToStayStillSetter();

        Player.Animator.speed = 0;
        Player.enabled = false;
        Player.Rigidbody.simulated = false;
        Player.Rigidbody.velocity = Vector2.zero;

        Player.SoundList.PlaySFX("SE_Die_02(Short)");
        Player.MaterialController.DisintegrateEffect.Play(0f, false);

        yield return new WaitUntil(() => Player.MaterialController.DisintegrateEffect.IsEffectDone);
        Player.MaterialController.DisintegrateEffect.ResetIsEffectDone();

        SceneContext.Current.InstantRespawn();  // move to check point & change to idle state

        _player = Player;
    }

    private IEnumerator ExitCoroutine()
    {
        _player.MaterialController.DisintegrateEffect.Play(0f, true);

        yield return new WaitUntil(() => _player.MaterialController.DisintegrateEffect.IsEffectDone);
        _player.MaterialController.DisintegrateEffect.ResetIsEffectDone();

        _player.Rigidbody.velocity = Vector2.zero;
        _player.Rigidbody.simulated = true;
        _player.enabled = true;
        _player.Animator.speed = 1;

        InputManager.Instance.ChangeToDefaultSetter();

        _player = null;
    }
}
