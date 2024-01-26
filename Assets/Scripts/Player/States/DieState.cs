using System.Collections;
using UnityEngine;

public class DieState : PlayerState
{
    [SerializeField] InputSetterScriptableObject _stayStillSetter;
    [SerializeField] float _moveUpDelay;
    [SerializeField] float _stayDuration;
    [SerializeField] float _spawnDuration;

    DisintegrateEffect _disintegrateEffect;
    public float DieDuration => _disintegrateEffect.Duration;
    public float SpawnDuration => _spawnDuration;

    private float _moveUpSpeed = 1f;
    private float _moveUpDuration = 3f;

    private float _previousGravityScale;

    private IEnumerator MoveUp()
    {
        yield return new WaitForSeconds(_moveUpDelay);

        // 위로 이동
        _previousGravityScale = Player.Rigidbody.gravityScale;
        Player.Rigidbody.gravityScale = 0f;
        Player.Rigidbody.velocity = Vector2.up * _moveUpSpeed;

        yield return new WaitForSeconds(_moveUpDuration);

        Player.Rigidbody.simulated = false;
        Player.enabled = false;

        yield return new WaitForSeconds(_stayDuration);

        _disintegrateEffect.Play();

        yield return new WaitForSeconds(_disintegrateEffect.Duration);

        // 씬 재시작
        Debug.Log("씬 재시작 타이밍");
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