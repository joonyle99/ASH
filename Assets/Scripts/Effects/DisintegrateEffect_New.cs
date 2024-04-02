using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisintegrateEffect_New : MonoBehaviour
{
    [SerializeField] private Material _disintegrateMaterial;
    [SerializeField] private float _duration;
    [SerializeField] private float _timeOffsetAfterParticle = 0.2f;
    [SerializeField] private ParticleHelper _particle;
    [SerializeField] private SpriteRenderer[] _spriteRenderers;

    public bool IsEffectDone { get; private set; } = false;
    public float Duration => _duration;

    public void Play(float delay = 0f)
    {
        StartCoroutine(ProgressCoroutine(delay));
    }

    IEnumerator ProgressCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Disintegrate Material Initialize
        foreach (var spriteRenderer in _spriteRenderers)
        {
            spriteRenderer.material = _disintegrateMaterial;
            spriteRenderer.material.SetFloat("_Progress", 0f);
        }

        // ParticleHelper
        _particle.transform.parent = null;
        _particle.transform.position = transform.position;
        _particle.gameObject.SetActive(true);

        yield return new WaitForSeconds(_timeOffsetAfterParticle);

        // Disintegrate Effect Progress
        float eTime = 0f;
        while (eTime < _duration)
        {
            float progress = 0f;
            Debug.Log(progress);
            foreach (var spriteRenderer in _spriteRenderers)
            {
                progress = eTime / _duration;
                spriteRenderer.material.SetFloat("_Progress", progress);
            }

            eTime += Time.deltaTime;
            yield return null;
        }

        IsEffectDone = true;
    }
}
