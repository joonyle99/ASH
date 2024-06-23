using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class DieState : PlayerState
{
    [Header("Die State")]
    [Space]

    [SerializeField] private float _dieEffectDelay = 0.3f;

    [Space]

    [SerializeField] private float _moveUpDuration = 1.5f;
    [SerializeField] private float _moveUpDistance = 4f;
    [SerializeField] private float _stayInAirDuration = 0.3f;

    protected override bool OnEnter()
    {
        Debug.Log("enter die state");

        StartCoroutine(EnterCoroutine());

        return true;
    }
    protected override bool OnUpdate()
    {

        return true;
    }
    protected override bool OnExit()
    {
        Debug.Log("exit die state");

        StartCoroutine(ExitCoroutine());

        return true;
    }

    private IEnumerator EnterCoroutine()
    {
        Player.IsDead = true;

        // stay still input setter
        InputManager.Instance.ChangeToStayStillSetter();

        // notice die sound
        Player.SoundList.PlaySFX("SE_Die_02(Short)");

        // move up effect
        Player.Animator.SetTrigger("Die");
        yield return new WaitForSeconds(_dieEffectDelay);
        yield return StartCoroutine(MoveUpEffectCoroutine());
        yield return new WaitForSeconds(_stayInAirDuration);

        // die effect
        Player.PlaySound_SE_Die_01();
        Player.MaterialController.DisintegrateEffect.Play(0f, false);
        yield return new WaitUntil(() => Player.MaterialController.DisintegrateEffect.IsEffectDone);
        Player.MaterialController.DisintegrateEffect.ResetIsEffectDone();

        // exit scene effect
        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();

        // restart scene
        // move to scene entrance passage
        var sceneName = SceneManager.GetActiveScene().name;
        var passageName = SceneContext.Current.EntrancePassage.PassageName;
        SceneChangeManager.Instance.ChangeToPlayableScene(sceneName, passageName);      // load scene
    }
    private IEnumerator ExitCoroutine()
    {
        yield return null;

        Player.Animator.SetTrigger("EndDie");

        Player.IsDead = false;

        InputManager.Instance.ChangeToDefaultSetter();
    }

    private IEnumerator MoveUpEffectCoroutine()
    {
        var playerRigidBody = Player.Rigidbody;

        // 1. preparing to move up
        playerRigidBody.gravityScale = 0f;
        playerRigidBody.velocity = Vector2.zero;

        // 2. set from & to position
        Vector2 currentPosition = playerRigidBody.position;
        Vector2 targetPosition = playerRigidBody.position + Vector2.up * _moveUpDistance;

        // 3. move from originalPos to targetPos by easing function
        var eTime = 0f;
        while (eTime < _moveUpDuration)
        {
            var nextFramePosition =
                Vector2.Lerp(currentPosition, targetPosition, Curves.EaseOut(eTime / _moveUpDuration));
            playerRigidBody.MovePosition(nextFramePosition);
            yield return null;
            eTime += Time.deltaTime;
        }

        // 4. pause player
        playerRigidBody.simulated = false;
        Player.enabled = false;
    }
}