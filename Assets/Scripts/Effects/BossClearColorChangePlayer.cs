using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossClearColorChangePlayer : MonoBehaviour
{
    [SerializeField] Material _material;
    [SerializeField] float _duration = 2f;

    BossClearColorChange[] _changeTargets; 

    void Awake()
    {
        _changeTargets = FindObjectsOfType<BossClearColorChange>();
        foreach (var target in _changeTargets)
        {
            target.Initialize(_material);
        }
    }
    private void Start()
    {
        PlayEffect();
    }
    public void PlayEffect()
    {
        StartCoroutine(PlayEffectCoroutine());
    }
    IEnumerator PlayEffectCoroutine()
    {
        float eTime = 0f;
        while (eTime < _duration)
        {
            yield return null;
            eTime += Time.deltaTime;

            foreach (var target in _changeTargets)
            {
                target.SetProgress(eTime / _duration);
            }
        }
    }
}
