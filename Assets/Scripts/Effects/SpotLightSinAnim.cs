using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpotLightSinAnim : MonoBehaviour
{
    [SerializeField] float _frequency = 1f;
    [SerializeField] float _scaleDiffAmount = 0.1f;

    float originalRadius;
    float eTime = 0f;

    Light2D _light;
    private void Awake()
    {
        _light = GetComponent<Light2D>();
        originalRadius = _light.pointLightOuterRadius;
    }
    private void Update()
    {
        eTime += Time.deltaTime;
        _light.pointLightOuterRadius = originalRadius  + Mathf.Sin(eTime * _frequency * Mathf.PI) * _scaleDiffAmount / 2;
    }
}
