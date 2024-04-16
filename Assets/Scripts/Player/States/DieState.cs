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

    DisintegrateEffect_Old disintegrateEffectOld;
    public float DieDuration => disintegrateEffectOld.Duration;
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

        Player.SoundList.PlaySFX("Disintegrate");
        disintegrateEffectOld.Play();

        yield return new WaitForSeconds(disintegrateEffectOld.Duration);

        // 씬 재시작
        yield return SceneContext.Current.SceneTransitionPlayer.ExitEffectCoroutine();
        string passageName = SceneContext.Current.EntrancePassage.PassageName;
        if (SceneContext.Current.EntrancePassage == null)
            passageName = "";
        SceneChangeManager.Instance.ChangeToPlayableScene(SceneManager.GetActiveScene().name, SceneContext.Current.EntrancePassage.PassageName);
    }

    void Awake()
    {
        disintegrateEffectOld = GetComponent<DisintegrateEffect_Old>();
    }

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
}