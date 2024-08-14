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
        return true;
    }

    private IEnumerator EnterCoroutine()
    {
        InputManager.Instance.ChangeToStayStillSetter();

        Player.SoundList.PlaySFX("SE_Die_02(Short)");

        Player.Animator.SetTrigger("Die");

        Player.IsDead = true;

        yield return new WaitForSeconds(_dieEffectDelay);

        yield return MoveUpEffectCoroutine();

        yield return new WaitForSeconds(_stayInAirDuration);

        Player.PlaySound_SE_Die_01();
        Player.MaterialController.DisintegrateEffect.Play();

        yield return new WaitUntil(() => Player.MaterialController.DisintegrateEffect.IsEffectDone);

        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();

        // 체력 초기화
        Player.CurHp = Player.MaxHp;

        SceneChangeManager.Instance.SceneChangeType = SceneChangeType.PlayerRespawn;

        var sceneName = PersistentDataManager.Instance.SavedPersistentData.SceneName;
        sceneName = sceneName == "" ? SceneManager.GetActiveScene().name : sceneName;
        var passageName = PersistentDataManager.Instance.SavedPersistentData.PassageName;
        passageName = passageName == "" ? SceneContext.Current.EntrancePassage.PassageName : passageName;

        SceneChangeManager.Instance.ChangeToPlayableScene(sceneName, passageName);
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
            var t = Curves.EaseOut(eTime / _moveUpDuration);
            var nextFramePosition = Vector2.Lerp(currentPosition, targetPosition, t);
            playerRigidBody.MovePosition(nextFramePosition);

            yield return null;
            eTime += Time.deltaTime;
        }

        // 4. pause player
        playerRigidBody.simulated = false;
        Player.enabled = false;
    }
}