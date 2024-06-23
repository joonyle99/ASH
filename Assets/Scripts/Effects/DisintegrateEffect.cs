using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MaterialController))]
public class DisintegrateEffect : MonoBehaviour
{
    [SerializeField] private Material _disintegrateMaterial;
    [SerializeField] private float _duration;

    [Space]

    [SerializeField] private ParticleHelper[] _particles;

    [field: Space]

    public bool IsEffectDone { get; private set; }

    private MaterialController _materialController;

    public Coroutine disintegrateCoroutine;

    private void Awake()
    {
        _materialController = GetComponent<MaterialController>();
    }

    public void Play(float delay = 0f, bool isRespawn = false)
    {
        // blink effect 실행 중이라면, 종료하고 disintegrate effect를 실행한다
        if (_materialController.BlinkEffect)
        {
            if (_materialController.BlinkEffect.blinkCoroutine != null)
            {
                _materialController.BlinkEffect.StopCoroutine(_materialController.BlinkEffect.blinkCoroutine);
                _materialController.BlinkEffect.blinkCoroutine = null;
            }
        }

        if (disintegrateCoroutine != null)
        {
            StopCoroutine(disintegrateCoroutine);
            disintegrateCoroutine = null;
        }

        disintegrateCoroutine = StartCoroutine(ProgressCoroutine(delay, isRespawn));
    }
    private IEnumerator ProgressCoroutine(float delay, bool isRespawn)
    {
        yield return new WaitForSeconds(delay);

        if (!isRespawn)
        {
            // Debug.Log($"Disintegrate Effect in {name}");

            // Particle System Control
            foreach (var particleHelper in _particles)
            {
                particleHelper.gameObject.SetActive(true);
                particleHelper.transform.parent = null;
                particleHelper.transform.position = transform.position;

                // Destroy Particle System
                Destroy(particleHelper.gameObject, particleHelper.GetLifeTime());
            }

            // Disintegrate Material Initialize
            InitMaterialAndProgressForDie();
        }
        else
        {
            // Debug.Log($"Respawn Effect in {name}");

            // Disintegrate Material Initialize
            InitMaterialAndProgressForRespawn();
        }

        // Disintegrate Effect Progress
        var eTime = 0f;
        while (eTime < _duration)
        {
            var ratio = !isRespawn ? Mathf.Clamp01(eTime / _duration) : Mathf.Clamp01(1f - eTime / _duration);
            _materialController.SetProgress("_Progress", ratio);

            yield return null;

            eTime += Time.deltaTime;
        }

        // 리스폰의 경우 초기화 작업이 필요하다
        if (isRespawn)
        {
            _materialController.SetProgress("_Progress", 1f);
            _materialController.InitMaterial();
        }

        IsEffectDone = true;

        disintegrateCoroutine = null;
    }

    public void ResetIsEffectDone()
    {
        IsEffectDone = false;
    }

    public void InitMaterialAndProgressForDie()
    {
        _materialController.SetMaterialAndProgress(_disintegrateMaterial, "_Progress", 0f);
    }
    public void InitMaterialAndProgressForRespawn()
    {
        _materialController.SetMaterialAndProgress(_disintegrateMaterial, "_Progress", 1f);
    }
}
