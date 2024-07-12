using Coffee.UIExtensions;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveFlameEffect : MonoBehaviour
{
    [Header("Particle")]
    [SerializeField]
    private UIParticle _largeFlame;

    [Header("Timer")]
    [SerializeField]
    private float _minDisplayTime = 1.0f;
    private float _effectStartTime = 0f;

    private void Awake()
    {
        Initialize();

        SaveAndLoader.OnSaveStarted += PlayFlameEffect;
        SaveAndLoader.OnSaveEnded += StopFlameEffectLogic;
    }

    private void Initialize()
    {
        _largeFlame.Clear();
    }

    private void PlayFlameEffect()
    {
        if (_largeFlame == null) return;

        _largeFlame.Play();
        _effectStartTime = Time.time;
    }

    private void StopFlameEffectLogic()
    {
        if(_largeFlame == null) return;

        float elapsedTime = Time.time - _effectStartTime;
        elapsedTime = elapsedTime < _minDisplayTime ? _minDisplayTime - elapsedTime : 0;

        StopAllCoroutines();
        StartCoroutine(StopFlameEffectTimer(elapsedTime));
    }

    private IEnumerator StopFlameEffectTimer(float duration)
    {
        yield return new WaitForSeconds(duration);

        _largeFlame.Clear();
        _effectStartTime = 0;
    }
}
