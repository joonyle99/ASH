using System.Collections;
using UnityEngine;

public class BlinkEffect : MonoBehaviour
{
    [SerializeField] private Material _material;
    [SerializeField] private float _interval = 0.08f;
    [SerializeField] private float _duration = 0.9f;
    [SerializeField] private bool _isBlinking = false;

    private PlayerBehaviour _player;
    private MonsterBehavior _monster;

    [SerializeField]
    private SpriteRenderer[] _spriteRenderers;  // blink target spriteRenderers
    private Material[] _originalMaterials;      // originalMaterial's count == spriteRenderer's count

    private Coroutine _blinkCoroutine;

    public SpriteRenderer[] SpriteRenderers => _spriteRenderers;

    void Awake()
    {
        // subject
        _player = GetComponent<PlayerBehaviour>();
        _monster = GetComponent<MonsterBehavior>();

        // 몬스터의 경우에만 자동으로 SpriteRenderer를 찾아서 넣어줌
        if (_monster)
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        // save original Materials
        _originalMaterials = new Material[_spriteRenderers.Length];
        for (int i = 0; i < _originalMaterials.Length; i++)
            _originalMaterials[i] = _spriteRenderers[i].material;
    }

    public void InitMaterial()
    {
        // put original Material to spriteRenderer
        for (int i = 0; i < _spriteRenderers.Length; i++)
            _spriteRenderers[i].material = _originalMaterials[i];
    }
    public void ChangeMaterial(Material material)
    {
        // change material to spriteRenderer
        foreach (var spriteRenderer in _spriteRenderers)
            spriteRenderer.material = material;
    }
    private IEnumerator BlinkCoroutine()
    {
        // first, change material to blink material
        ChangeMaterial(_material);

        _isBlinking = true;
        if (_player) _player.IsGodMode = _isBlinking;

        // second, blink
        float startTime = Time.time;
        while (startTime + _duration > Time.time)
        {
            foreach (var spriteRenderer in _spriteRenderers)
                spriteRenderer.material.SetFloat("_FlashAmount", 0.3f);

            yield return new WaitForSeconds(_interval);

            foreach (var spriteRenderer in _spriteRenderers)
                spriteRenderer.material.SetFloat("_FlashAmount", 0f);

            yield return new WaitForSeconds(_interval);
        }

        _isBlinking = false;
        if (_player) _player.IsGodMode = _isBlinking;

        if ((_player && _player.IsDead) || (_monster && _monster.IsDead))
            yield break;

        // third, change material to original material
        InitMaterial();
    }
    public void Play()
    {
        if (_blinkCoroutine != null)
            StopCoroutine(_blinkCoroutine);

        _blinkCoroutine = StartCoroutine(BlinkCoroutine());
    }
}
