using System.Collections;
using UnityEngine;

public class BlinkEffect : MonoBehaviour
{
    [SerializeField] private Material _material;
    [SerializeField] private float _interval = 0.08f;
    [SerializeField] private float _duration = 0.9f;
    [SerializeField] private bool _isBlinking = false;

    private MonsterBehavior _monster;
    private SpriteRenderer[] _spriteRenderers;
    private Material[] _originalMaterials;
    private Coroutine _blinkCoroutine;

    void Awake()
    {
        _monster = GetComponent<MonsterBehavior>();

        // spriteRenderers
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        // materials
        _originalMaterials = new Material[_spriteRenderers.Length];
        for (int i = 0; i < _originalMaterials.Length; i++)
            _originalMaterials[i] = _spriteRenderers[i].material;
    }

    private void InitMaterial()
    {
        for (int i = 0; i < _spriteRenderers.Length; i++)
            _spriteRenderers[i].material = _originalMaterials[i];
    }
    private void ChangeMaterial(Material material)
    {
        for (int i = 0; i < _spriteRenderers.Length; i++)
            _spriteRenderers[i].material = material;
    }
    private IEnumerator BlinkCoroutine()
    {
        ChangeMaterial(_material);

        _isBlinking = true;

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

        if (_monster.IsDead) yield break;

        InitMaterial();
    }
    public void Play()
    {
        if (_blinkCoroutine != null)
            StopCoroutine(_blinkCoroutine);

        _blinkCoroutine = StartCoroutine(BlinkCoroutine());
    }
}
