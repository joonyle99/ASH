using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveFlameEffect : MonoBehaviour
{
    [Header("Particle")]
    [SerializeField]
    private ParticleSystem _particleSystem;

    private void Awake()
    {
        if (_particleSystem == null) return;

        SaveAndLoader.OnSaveStarted += PlayFlameEffect;
        SaveAndLoader.OnSaveEnded += StopFlameEffect;
    }
    private void PlayFlameEffect()
    {
        if (_particleSystem == null) return;

        _particleSystem.Play();
    }

    private void StopFlameEffect()
    {
        if( _particleSystem == null ) return;

        _particleSystem.Stop();
    }
}
