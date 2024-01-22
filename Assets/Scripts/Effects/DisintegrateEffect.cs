using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisintegrateEffect : MonoBehaviour
{
    [SerializeField] Material _disintegrateMaterial;
    [SerializeField] float _duration;
    [SerializeField] float _timeOffsetAfterParticle = 0.2f;
    [SerializeField] ParticleHelper _particle;

    [SerializeField] Range _heightRange;

    [ContextMenuItem("Get all", "GetAllSpriteRenderers")]
    [SerializeField] SpriteRenderer [] _spriteRenderers;

    public bool IsEffectDone { get; private set; } = false;

    void GetAllSpriteRenderers()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }
    public void Play(float delay = 0f)
    {
        StartCoroutine(ProgressCoroutine(delay));
    }
    IEnumerator ProgressCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var renderer in _spriteRenderers)
        {
            renderer.material = _disintegrateMaterial;
            renderer.material.SetFloat("_Progress", 0);
            renderer.material.SetFloat("_MinY", transform.position.y + _heightRange.Start);
            renderer.material.SetFloat("_MaxY", transform.position.y + _heightRange.End);
        }
        _particle.transform.parent = null;
        _particle.transform.position = transform.position;
        _particle.gameObject.SetActive(true);
        yield return new WaitForSeconds(_timeOffsetAfterParticle);
        float eTime = 0f;
        while (eTime < _duration)
        {
            yield return null;
            foreach (var renderer in _spriteRenderers)
            {
                renderer.material.SetFloat("_Progress", eTime / _duration);
                renderer.material.SetFloat("_MinY", transform.position.y + _heightRange.Start);
                renderer.material.SetFloat("_MaxY", transform.position.y + _heightRange.End);
            }
            eTime += Time.deltaTime;
        }
        IsEffectDone = true;
    }
}
