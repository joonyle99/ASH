using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisintegrateEffect_New : MonoBehaviour
{
    [SerializeField] private Material _disintegrateMaterial;
    [SerializeField] private float _duration;
    [SerializeField] private float _timeOffsetAfterParticle = 0.2f;
    [SerializeField] private ParticleHelper[] _particles;

    [field: Space]

    [field: SerializeField]
    public SpriteRenderer[] SpriteRenderers
    {
        get;
        set;
    }
    public Material[] OriginalMaterials
    {
        get;
        private set;
    }

    public bool IsEffectDone { get; private set; }

    private void Awake()
    {
        // save original Materials
        OriginalMaterials = new Material[SpriteRenderers.Length];
        for (int i = 0; i < OriginalMaterials.Length; i++)
            OriginalMaterials[i] = SpriteRenderers[i].material;
    }

    public void Play(float delay = 0f)
    {
        StartCoroutine(ProgressCoroutine(delay));
    }
    IEnumerator ProgressCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        // ParticleHelper
        foreach (var particleHelper in _particles)
        {
            particleHelper.transform.parent = null;
            particleHelper.transform.position = transform.position;
            particleHelper.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            particleHelper.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(_timeOffsetAfterParticle);

        // Disintegrate Material Initialize
        foreach (var spriteRenderer in SpriteRenderers)
        {
            spriteRenderer.material = _disintegrateMaterial;
            spriteRenderer.material.SetFloat("_Progress", 0f);
        }

        // Disintegrate Effect Progress
        float eTime = 0f;
        while (eTime < _duration)
        {
            yield return null;
            eTime += Time.deltaTime;

            foreach (var spriteRenderer in SpriteRenderers)
            {
                spriteRenderer.material.SetFloat("_Progress", eTime / _duration);
            }
        }

        yield return new WaitForSeconds(delay);

        IsEffectDone = true;
    }

    public void Revert()
    {
        IsEffectDone = false;

        // ParticleHelper
        foreach (var particleHelper in _particles)
        {
            particleHelper.gameObject.SetActive(false);
            particleHelper.transform.parent = transform;
            particleHelper.transform.position = Vector3.zero;
        }

        // Disintegrate Material Initialize
        foreach (var spriteRenderer in SpriteRenderers)
        {
            spriteRenderer.material.SetFloat("_Progress", 0f);
        }

        // Init SpriteRenderers
        for (int i = 0; i < SpriteRenderers.Length; i++)
        {
            SpriteRenderers[i].material = OriginalMaterials[i];
        }
    }
}
