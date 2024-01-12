using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightIcon : MonoBehaviour
{
    public enum Shape
    {
        Wind, Fire, Light   
    }
    [SerializeField] Shape _shape;
    [SerializeField] bool _lightOn = true;

    [SerializeField] Material _defaultMaterial;
    [SerializeField] Material _lightMaterial;

    [SerializeField] Sprite[] _offImages;
    [SerializeField] Sprite[] _onImages;

    SpriteRenderer _spriteRenderer;

    private void Awake()
    {
    }
    private void OnValidate()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_lightOn)
        {
            _spriteRenderer.material = _lightMaterial;
            _spriteRenderer.sprite = _onImages[(int)_shape];
        }
        else
        {
            _spriteRenderer.material = _defaultMaterial;
            _spriteRenderer.sprite = _offImages[(int)_shape];
        }
    }
}
