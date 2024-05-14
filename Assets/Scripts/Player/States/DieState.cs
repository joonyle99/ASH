using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class DieState : PlayerState
{
    [Header("Die State")]
    [Space]

    [SerializeField] InputSetterScriptableObject _stayStillSetter;

    [Space]

    [SerializeField] private float _moveUpDuration = 1.5f;
    [SerializeField] private float _moveUpDistance = 4f;

    [Space]

    [SerializeField] private float _moveUpDelay;
    [SerializeField] private float _stayDuration;

    private float _previousGravityScale;

    protected override bool OnEnter()
    {
        Animator.SetTrigger("Die");

        Player.IsDead = true;

        InputManager.Instance.ChangeInputSetter(_stayStillSetter);

        StartCoroutine(MoveUp());

        return true;
    }
    protected override bool OnUpdate()
    {

        return true;
    }
    protected override bool OnFixedUpdate()
    {

        return true;
    }
    protected override bool OnExit()
    {
        /*
        Player.IsDead = false;
        Animator.SetBool("IsDead", Player.IsDead);

        InputManager.Instance.ChangeToDefaultSetter();
        */

        return true;
    }

    private IEnumerator MoveUp()
    {
        yield return new WaitForSeconds(_moveUpDelay);

        var playerRigidBody = Player.Rigidbody;

        // ���� �̵�
        _previousGravityScale = playerRigidBody.gravityScale;
        playerRigidBody.gravityScale = 0f;
        playerRigidBody.velocity = Vector2.zero;

        Vector2 originalPos = playerRigidBody.position;
        Vector2 targetPos = playerRigidBody.position + Vector2.up * _moveUpDistance;

        var eTime = 0f;
        while (eTime < _moveUpDuration)
        {
            yield return null;
            eTime += Time.deltaTime;
            playerRigidBody.MovePosition(Vector2.Lerp(originalPos, targetPos, Curves.EaseOut(eTime / _moveUpDuration)));
        }

        // �÷��̾� ����
        playerRigidBody.simulated = false;
        Player.enabled = false;

        yield return new WaitForSeconds(_stayDuration);

        Player.SoundList.PlaySFX("Disintegrate");
        Player.MaterialController.DisintegrateEffect.Play();

        yield return new WaitUntil(() => Player.MaterialController.DisintegrateEffect.IsEffectDone);

        // TODO: �� �����
        yield return SceneContext.Current.SceneTransitionPlayer.ExitEffectCoroutine();

        /*
        if (SceneContext.Current.EntrancePassage == null)
        {
           string passageName = SceneContext.Current.EntrancePassage.PassageName;
            passageName = "";
        }
        */

        SceneChangeManager.Instance.ChangeToPlayableScene(SceneManager.GetActiveScene().name, SceneContext.Current.EntrancePassage.PassageName);
    }
}