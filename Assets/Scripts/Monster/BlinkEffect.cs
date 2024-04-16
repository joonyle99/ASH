using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MaterialManager))]
public class BlinkEffect : MonoBehaviour
{
    [SerializeField] private Material _material;
    [SerializeField] private float _interval = 0.08f;
    [SerializeField] private float _duration = 0.9f;
    [SerializeField] private bool _isBlinking = false;

    private PlayerBehaviour _player;
    private MonsterBehavior _monster;

    private Coroutine _blinkCoroutine;
    private MaterialManager _materialManager;

    void Awake()
    {
        // subject
        _player = GetComponent<PlayerBehaviour>();
        _monster = GetComponent<MonsterBehavior>();

        _materialManager = GetComponent<MaterialManager>();
    }

    private IEnumerator BlinkCoroutine()
    {
        // first, change material to blink material
        _materialManager.SetMaterial(_material);

        _isBlinking = true;
        if (_player) _player.IsGodMode = _isBlinking;

        // second, blink
        float startTime = Time.time;
        while (startTime + _duration > Time.time)
        {
            _materialManager.SetProgress("_FlashAmount", 0.3f);
            yield return new WaitForSeconds(_interval);

            _materialManager.SetProgress("_FlashAmount", 0f);
            yield return new WaitForSeconds(_interval);
        }

        _isBlinking = false;
        if (_player) _player.IsGodMode = _isBlinking;

        if ((_player && _player.IsDead) || (_monster && _monster.IsDead))
            yield break;

        // third, change material to original material
        _materialManager.InitMaterial();
    }
    public void Play()
    {
        if (_blinkCoroutine != null)
            StopCoroutine(_blinkCoroutine);

        _blinkCoroutine = StartCoroutine(BlinkCoroutine());
    }
}
