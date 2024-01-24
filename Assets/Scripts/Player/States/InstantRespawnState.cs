using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class InstantRespawnState : PlayerState
{
    [SerializeField] InputSetterScriptableObject _stayStillSetter;
    [SerializeField] float _spawnDuration;
    SpriteRenderer[] _spriteRenderers;

    DisintegrateEffect _disintegrateEffect;
    public float DieDuration => _disintegrateEffect.Duration;
    public float SpawnDuration => _spawnDuration;
    void Awake()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
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
    IEnumerator SpawnCoroutine()
    {
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            _spriteRenderers[i].material = Player.OriginalMaterials[i];
        }
        Player.enabled = true;
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
        Player.Rigidbody.simulated = true;
        Player.Alive();
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
