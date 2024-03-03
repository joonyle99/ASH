using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShapeLightFalloffAnim : MonoBehaviour
{
    [SerializeField] float _speed = 1f;
    [SerializeField] float _power = 1f;

    Light2D _light;
    float _originalFallOff;
    private void Awake()
    {
        _light = GetComponent<Light2D>();
        _originalFallOff = _light.shapeLightFalloffSize;
    }
    private void Update()
    {
        _light.shapeLightFalloffSize = Mathf.Lerp(_originalFallOff, _power + _originalFallOff, (1 + Mathf.Sin(Time.time * _speed * Mathf.PI)) / 2f);
    }
}
