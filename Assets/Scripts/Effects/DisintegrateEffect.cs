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

    private MaterialController materialController;

    private void Awake()
    {
        materialController = GetComponent<MaterialController>();
    }

    public void Play(float delay = 0f, bool isRespawn = false)
    {
        StartCoroutine(ProgressCoroutine(delay, isRespawn));
    }
    private IEnumerator ProgressCoroutine(float delay, bool isRespawn)
    {
        yield return new WaitForSeconds(delay);

        if (!isRespawn)
        {
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
            materialController.SetMaterialAndProgress(_disintegrateMaterial, "_Progress", 0f);
        }
        else
        {
            // Disintegrate Material Initialize
            materialController.SetMaterialAndProgress(_disintegrateMaterial, "_Progress", 1f);
        }

        // Disintegrate Effect Progress
        var eTime = 0f;
        while (eTime < _duration)
        {
            yield return null;
            eTime += Time.deltaTime;

            var ratio = !isRespawn ? Mathf.Clamp01(eTime / _duration) : Mathf.Clamp01(1f - eTime / _duration);
            materialController.SetProgress("_Progress", ratio);
        }

        // �������� ��� �ʱ�ȭ �۾��� �ʿ��ϴ�
        if (isRespawn)
        {
            materialController.SetProgress("_Progress", 1f);
            materialController.InitMaterial();
        }

        IsEffectDone = true;
    }

    public void ResetIsEffectDone()
    {
        IsEffectDone = false;
    }
}
