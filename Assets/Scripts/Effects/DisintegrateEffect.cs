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

    public void Play(float delay = 0f)
    {
        StartCoroutine(ProgressCoroutine(delay));
    }
    private IEnumerator ProgressCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

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

        // Disintegrate Effect Progress
        var eTime = 0f;
        while (eTime < _duration)
        {
            yield return null;
            eTime += Time.deltaTime;

            materialController.SetProgress("_Progress", eTime / _duration);
        }

        IsEffectDone = true;
    }
}
