using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MaterialController))]
public class BlinkEffect : MonoBehaviour
{
    [SerializeField] private Material _material;
    [SerializeField] private float _interval = 0.08f;
    [SerializeField] private float _duration = 0.9f;
    [SerializeField] private bool _isBlinking;
    public bool IsBlinking => _isBlinking;

    private PlayerBehaviour _player;
    private MonsterBehaviour _monster;

    private MaterialController _materialController;

    public Coroutine blinkCoroutine;

    private void Awake()
    {
        // subject
        _player = GetComponent<PlayerBehaviour>();
        _monster = GetComponent<MonsterBehaviour>();

        _materialController = GetComponent<MaterialController>();
    }

    public void Play()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        blinkCoroutine = StartCoroutine(BlinkCoroutine());
    }
    private IEnumerator BlinkCoroutine()
    {
        // first, change material to blink material
        _materialController.SetMaterial(_material);

        _isBlinking = true;
        if (_player) _player.IsGodMode = _isBlinking;

        // second, blink
        var startTime = Time.time;
        while (startTime + _duration > Time.time)
        {
            _materialController.SetProgress("_FlashAmount", 0.3f);
            yield return new WaitForSeconds(_interval);

            _materialController.SetProgress("_FlashAmount", 0f);
            yield return new WaitForSeconds(_interval);
        }

        _isBlinking = false;
        if (_player) _player.IsGodMode = _isBlinking;

        // blink effect 도중에 사망하면 머터리얼을 초기화하지 않음
        if ((_player && _player.IsDead) || (_monster && _monster.IsDead))
            yield break;

        // third, change material to original material
        _materialController.InitMaterial();

        // when effect ended, coroutine is also ended
        blinkCoroutine = null;
    }
}
