using System.Collections;
using UnityEngine;

public class InstantRespawnState : PlayerState
{
    [Header("Instant Respawn State")]
    [Space]

    [SerializeField] private InputSetterScriptableObject _stayStillSetter;
    [SerializeField] private float _spawnDuration;

    private PlayerBehaviour _player;

    protected override bool OnEnter()
    {
        Debug.Log("Instant Respawn Enter");

        StartCoroutine(EnterCoroutine());

        return true;
    }
    protected override bool OnUpdate()
    {

        return true;
    }
    protected override bool OnExit()
    {
        Debug.Log("Instant Respawn Exit");

        StartCoroutine(ExitCoroutine());

        return true;
    }

    private IEnumerator EnterCoroutine()
    {
        InputManager.Instance.ChangeInputSetter(_stayStillSetter);

        Animator.speed = 0;
        Player.enabled = false;
        Player.Rigidbody.simulated = false;
        Player.Rigidbody.velocity = Vector2.zero;

        Player.SoundList.PlaySFX("Disintegrate");
        Player.MaterialController.DisintegrateEffect.Play(0f, false);

        yield return new WaitUntil(() => Player.MaterialController.DisintegrateEffect.IsEffectDone);
        Player.MaterialController.DisintegrateEffect.ResetIsEffectDone();

        SceneContext.Current.InstantRespawn();

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
