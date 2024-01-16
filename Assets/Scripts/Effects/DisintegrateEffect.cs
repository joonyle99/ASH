using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisintegrateEffect : MonoBehaviour
{
    [SerializeField] Material _disintegrateMaterial;
    [SerializeField] float _duration;
    [SerializeField] float _timeOffsetAfterParticle = 0.2f;
    [SerializeField] ParticleHelper _particle;

    [ContextMenuItem("Get all", "GetAllSpriteRenderers")]
    [SerializeField] SpriteRenderer [] _spriteRenderers;

    void GetAllSpriteRenderers()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }
    public void Play()
    {
        foreach(var renderer in _spriteRenderers)
        {
            renderer.material = _disintegrateMaterial;
        }
        StartCoroutine(ProgressCoroutine());
    }
    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Q))
        {
            Play();
        }
    }
    IEnumerator ProgressCoroutine()
    {
        _particle.gameObject.SetActive(true);
        yield return new WaitForSeconds(_timeOffsetAfterParticle);
        float eTime = 0f;
        while (eTime < _duration)
        {
            yield return null;
            foreach (var renderer in _spriteRenderers)
            {
                renderer.material.SetFloat("_Progress", eTime / _duration);
            }
            eTime += Time.deltaTime;
        }
        
    }
}
