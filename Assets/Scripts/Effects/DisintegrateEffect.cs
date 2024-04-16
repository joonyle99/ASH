using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MaterialManager))]
public class DisintegrateEffect : MonoBehaviour
{
    [SerializeField] private Material _disintegrateMaterial;
    [SerializeField] private float _duration;

    [Space]

    [SerializeField] private ParticleHelper[] _particles;

    [field: Space]

    public bool IsEffectDone { get; private set; }

    private MaterialManager materialManager;

    private void Awake()
    {
        materialManager = GetComponent<MaterialManager>();
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
            // particleHelper.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        }

        // Disintegrate Material Initialize
        materialManager.SetMaterialAndProgress(_disintegrateMaterial, "_Progress", 0f);

        // Disintegrate Effect Progress
        float eTime = 0f;
        while (eTime < _duration)
        {
            yield return null;
            eTime += Time.deltaTime;

            materialManager.SetProgress("_Progress", eTime / _duration);
        }

        yield return new WaitForSeconds(delay);

        IsEffectDone = true;
    }
    public void Revert()
    {
        IsEffectDone = false;

        // Disintegrate Material Initialize
        materialManager.SetProgress("_Progress", 0f);

        // Particle System Control
        foreach (var particleHelper in _particles)
        {
            particleHelper.transform.parent = transform;
            particleHelper.transform.position = Vector3.zero;
            particleHelper.gameObject.SetActive(false);
        }

        // Init SpriteRenderers Material
        materialManager.InitMaterial();
    }
}
