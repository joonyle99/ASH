using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class DieState : PlayerState
{
    [SerializeField] InputSetterScriptableObject _stayStillSetter;
    [SerializeField] float _moveUpDelay;
    [SerializeField] float _stayDuration;
    [SerializeField] float _spawnDuration;

    DisintegrateEffect _disintegrateEffect;
    public float DieDuration => _disintegrateEffect.Duration;
    public float SpawnDuration => _spawnDuration;

    [SerializeField] float _moveUpDistance = 4f;
    [SerializeField] float _moveUpDuration = 1.5f;

    private float _previousGravityScale;

    private IEnumerator MoveUp()
    {
        yield return new WaitForSeconds(_moveUpDelay);

        // 위로 이동
        _previousGravityScale = Player.Rigidbody.gravityScale;
        Player.Rigidbody.gravityScale = 0f;
        Player.Rigidbody.velocity = Vector2.zero;

        Vector2 originalPos = Player.Rigidbody.position;
        Vector2 targetPos = Player.Rigidbody.position + Vector2.up * _moveUpDistance;
        float eTime = 0f;
        while(eTime < _moveUpDuration)
        {
            Player.Rigidbody.MovePosition(Vector2.Lerp(originalPos, targetPos, Curves.EaseOut(eTime / _moveUpDuration)));
            yield return null;
            eTime += Time.deltaTime;
        }

        Player.Rigidbody.simulated = false;
        Player.enabled = false;

        yield return new WaitForSeconds(_stayDuration);

        _disintegrateEffect.Play();

        yield return new WaitForSeconds(_disintegrateEffect.Duration);

        // 씬 재시작
        yield return SceneContext.Current.SceneTransitionPlayer.ExitEffectCoroutine();
        SceneChangeManager.Instance.ChangeToPlayableScene(SceneManager.GetActiveScene().name, SceneContext.Current.EntrancePassage.PassageName);
    }

    void Awake()
    {
        _disintegrateEffect = GetComponent<DisintegrateEffect>();
    }

    protected override void OnEnter()
    {
        Animator.SetTrigger("Die");

        Player.IsDead = true;
        Animator.SetBool("IsDead", Player.IsDead);

        InputManager.Instance.ChangeInputSetter(_stayStillSetter);

        StartCoroutine(MoveUp());
    }
    protected override void OnUpdate()
    {

    }
    protected override void OnFixedUpdate()
    {

    }
    protected override void OnExit()
    {
        /*
        Player.IsDead = false;
        Animator.SetBool("IsDead", Player.IsDead);

        InputManager.Instance.ChangeToDefaultSetter();
        */
    }
}