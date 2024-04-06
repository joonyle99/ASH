using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisintegrateEffect_New : MonoBehaviour
{
    [SerializeField] private Material _disintegrateMaterial;
    [SerializeField] private float _duration;
    [SerializeField] private float _timeOffsetAfterParticle = 0.2f;
    [SerializeField] private ParticleHelper _particle;

    [field: Space]

    [field: SerializeField]
    public SpriteRenderer[] SpriteRenderers
    {
        get;
        set;
    }

    public bool IsEffectDone { get; private set; } = false;
    public float Duration => _duration;

    public void Play(float delay = 0f)
    {
        StartCoroutine(ProgressCoroutine(delay));
    }

    IEnumerator ProgressCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        // ParticleHelper
        _particle.transform.parent = null;
        _particle.transform.position = transform.position;
        _particle.gameObject.SetActive(true);

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
            foreach (var spriteRenderer in SpriteRenderers)
            {
                spriteRenderer.material.SetFloat("_Progress", eTime / _duration);
            }

            eTime += Time.deltaTime;
            yield return null;
        }

        IsEffectDone = true;
    }
}
