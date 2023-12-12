using System.Collections;
using UnityEngine;
public class InstantRespawnState : PlayerState
{
    [SerializeField] InputSetterScriptableObject _stayStillSetter;
    [SerializeField] float _dieDuration;
    [SerializeField] float _spawnDuration;
    SpriteRenderer[] _spriteRenderers;

    public float DieDuration => _dieDuration;
    public float SpawnDuration => _spawnDuration;
    void Awake()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }
    protected override void OnEnter()
    {
        Player.enabled = false;
        Animator.speed = 0;
        Player.Rigidbody.simulated = false;
        InputManager.Instance.ChangeInputSetter(_stayStillSetter);
        StartCoroutine(DieCoroutine());
    }
    IEnumerator DieCoroutine()
    {
        float eTime = 0f;
        while (eTime < _dieDuration)
        {
            foreach (var renderer in _spriteRenderers)
            {
                Color color = renderer.color;
                color.a = Mathf.Lerp(1, 0, eTime / _dieDuration);
                renderer.color = color;
            }
            yield return null;
            eTime += Time.deltaTime;
        }
        foreach (var renderer in _spriteRenderers)
        {
            Color color = renderer.color;
            color.a = 0;
            renderer.color = color;
        }
    }
    IEnumerator SpawnCoroutine()
    {
        Player.enabled = true;
        Player.Rigidbody.simulated = true;
        Player.Rigidbody.velocity = Vector2.zero;
        Animator.speed = 1;

        float eTime = 0f;
        while (eTime < _spawnDuration)
        {
            foreach (var renderer in _spriteRenderers)
            {
                Color color = renderer.color;
                color.a = Mathf.Lerp(0, 1, eTime / _spawnDuration);
                renderer.color = color;
            }
            yield return null;
            eTime += Time.deltaTime;
        }
        foreach (var renderer in _spriteRenderers)
        {
            Color color = renderer.color;
            color.a = 1;
            renderer.color = color;
        }
        ChangeState<IdleState>();
    }
    public void Respawn()
    {
        StartCoroutine(SpawnCoroutine());
    }
    protected override void OnUpdate()
    {
    }

    protected override void OnExit()
    {
        InputManager.Instance.ChangeToDefaultSetter();
    }
}
