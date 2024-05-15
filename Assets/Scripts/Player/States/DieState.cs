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

    [SerializeField] private float _dieEffectDelay = 0.3f;

    [Space]

    [SerializeField] private float _moveUpDuration = 1.5f;
    [SerializeField] private float _moveUpDistance = 4f;
    [SerializeField] private float _stayInAirDuration = 0.3f;

    private float _previousGravityScale;

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
    protected override bool OnFixedUpdate()
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
        _player = Player;

        InputManager.Instance.ChangeInputSetter(_stayStillSetter);

        _player.SoundList.PlaySFX("SE_Die_02(Short)");

        _player.Animator.SetTrigger("Die");

        _player.IsDead = true;

        yield return new WaitForSeconds(_dieEffectDelay);

        yield return StartCoroutine(MoveUpEffectCoroutine());

        yield return new WaitForSeconds(_stayInAirDuration);

        _player.PlaySound_SE_Die_01();
        _player.MaterialController.DisintegrateEffect.Play();

        yield return new WaitUntil(() => _player.MaterialController.DisintegrateEffect.IsEffectDone);

        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();

        Debug.Log($"scene name: {SceneManager.GetActiveScene().name}");
        Debug.Log($"passage name: {SceneContext.Current.EntrancePassage.PassageName}"); // TODO: 여기에 접근이 안된다

        // SceneChangeManager.Instance.ChangeToPlayableScene(, );

        /*
        if (SceneContext.Current.EntrancePassage == null)
        {
           var passageName = SceneContext.Current.EntrancePassage.PassageName;
           passageName = "";
        }
        */
    }

    private IEnumerator ExitCoroutine()
    {
        yield return null;

        _player.Animator.SetTrigger("EndDie");

        _player.IsDead = false;

        InputManager.Instance.ChangeToDefaultSetter();

        _player = null;
    }

    private IEnumerator MoveUpEffectCoroutine()
    {
        var playerRigidBody = Player.Rigidbody;

        // 1. preparing to move up
        _previousGravityScale = playerRigidBody.gravityScale;
        playerRigidBody.gravityScale = 0f;
        playerRigidBody.velocity = Vector2.zero;

        // 2. set from & to position
        Vector2 currentPosition = playerRigidBody.position;
        Vector2 targetPosition = playerRigidBody.position + Vector2.up * _moveUpDistance;

        // 3. move from originalPos to targetPos by easing function
        var eTime = 0f;
        while (eTime < _moveUpDuration)
        {
            yield return null;
            eTime += Time.deltaTime;
            var nextFramePosition =
                Vector2.Lerp(currentPosition, targetPosition, Curves.EaseOut(eTime / _moveUpDuration));
            playerRigidBody.MovePosition(nextFramePosition);
        }

        // 4. pause player
        playerRigidBody.simulated = false;
        Player.enabled = false;
    }
}