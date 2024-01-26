using System.Collections;
using UnityEngine;

public class InstantRespawnState : PlayerState
{
    [SerializeField] InputSetterScriptableObject _stayStillSetter;
    [SerializeField] float _spawnDuration;

    DisintegrateEffect _disintegrateEffect;
    public float DieDuration => _disintegrateEffect.Duration;
    public float SpawnDuration => _spawnDuration;

    void Awake()
    {
        _disintegrateEffect = GetComponent<DisintegrateEffect>();
    }

    protected override void OnEnter()
    {
        Player.Rigidbody.simulated = false;
        Player.enabled = false;
        Animator.speed = 0;
        InputManager.Instance.ChangeInputSetter(_stayStillSetter);
        _disintegrateEffect.Play();
    }
    protected override void OnUpdate()
    {

    }
    protected override void OnExit()
    {
        InputManager.Instance.ChangeToDefaultSetter();
    }

    IEnumerator SpawnCoroutine()
    {
        Player.InitMaterial();

        Player.enabled = true;
        Player.Rigidbody.velocity = Vector2.zero;
        Animator.speed = 1;

        float eTime = 0f;
        while (eTime < _spawnDuration)
        {
            foreach (var renderer in Player.SpriteRenderers)
            {
                Color color = renderer.color;
                color.a = Mathf.Lerp(0, 1, eTime / _spawnDuration);
                renderer.color = color;
            }
            yield return null;
            eTime += Time.deltaTime;
        }

        Player.InitSpriteRendererAlpha();

        Player.Rigidbody.simulated = true;

        ChangeState<IdleState>();

        yield return null;
    }
    public void Respawn()
    {
        StartCoroutine(SpawnCoroutine());
    }
}
