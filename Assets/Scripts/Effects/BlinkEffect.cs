using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MaterialController))]
public class BlinkEffect : MonoBehaviour
{
    [SerializeField] private Material _material;
    [SerializeField] private float _interval = 0.08f;
    [SerializeField] private float _duration = 0.9f;
    [SerializeField] private bool _isBlinking;

    private PlayerBehaviour _player;
    private MonsterBehavior _monster;

    private Coroutine _blinkCoroutine;
    private MaterialController materialController;

    private void Awake()
    {
        // subject
        _player = GetComponent<PlayerBehaviour>();
        _monster = GetComponent<MonsterBehavior>();

        materialController = GetComponent<MaterialController>();
    }

    public void Play()
    {
        // blink effect 실행 도중이면 중지하고 다시 시작

        if (_blinkCoroutine != null)
            StopCoroutine(_blinkCoroutine);

        _blinkCoroutine = StartCoroutine(BlinkCoroutine());
    }
    private IEnumerator BlinkCoroutine()
    {
        // first, change material to blink material
        materialController.SetMaterial(_material);

        _isBlinking = true;
        if (_player) _player.IsGodMode = _isBlinking;

        // second, blink
        var startTime = Time.time;
        while (startTime + _duration > Time.time)
        {
            materialController.SetProgress("_FlashAmount", 0.3f);
            yield return new WaitForSeconds(_interval);

            materialController.SetProgress("_FlashAmount", 0f);
            yield return new WaitForSeconds(_interval);
        }

        _isBlinking = false;
        if (_player) _player.IsGodMode = _isBlinking;

        // blink effect 도중에 사망하면 머터리얼을 초기화하지 않음
        if ((_player && _player.IsDead) || (_monster && _monster.IsDead))
            yield break;

        // third, change material to original material
        materialController.InitMaterial();
    }
}
