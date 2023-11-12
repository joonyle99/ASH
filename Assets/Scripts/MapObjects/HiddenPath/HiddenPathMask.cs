using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HiddenPathMask : MonoBehaviour
{
    public enum Direction
    {
        Left = 0, Right= 1, Up=2, Down=3
    }

    Direction _swipeDirection;
    [SerializeField] float _swipeDuration;
    [SerializeField] [HideInInspector] Rect _bounds;

    SpriteRenderer _spriteRenderer;
    float _maskBoundInit;
    float _maskBoundTarget;

    bool _allowSwipe = true;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        InitShaderValues();
    }

    void InitShaderValues()
    {
        _bounds = new Rect(transform.position - transform.lossyScale / 2, transform.lossyScale);
        _swipeDirection = (Direction)_spriteRenderer.material.GetFloat("_SWIPEDIRECTION");
        //_spriteRenderer.material.SetKeyword(new LocalKeyword(_spriteRenderer.material.shader, "_SWIPEDIRECTION_UP"), true);
        //_spriteRenderer.material.SetFloat("_SWIPEDIRECTION", (int)_swipeDirection);
        float gradientSize = _spriteRenderer.material.GetFloat("_GradientSize");
        if (_swipeDirection == Direction.Right)
        {
            _maskBoundInit = _bounds.xMin - gradientSize;
            _maskBoundTarget = _bounds.xMax;
        }
        if (_swipeDirection == Direction.Left)
        {
            _maskBoundInit = _bounds.xMax + gradientSize;
            _maskBoundTarget = _bounds.xMin;
        }
        if (_swipeDirection == Direction.Up)
        {
            _maskBoundInit = _bounds.yMin - gradientSize;
            _maskBoundTarget = _bounds.yMax;
        }
        if (_swipeDirection == Direction.Down)
        {
            _maskBoundInit = _bounds.yMax + gradientSize;
            _maskBoundTarget = _bounds.yMin;
        }
        _spriteRenderer.material.SetFloat("_MaskBound", _maskBoundInit);
    }
    public void OnLightCaptured()
    {
        if (_allowSwipe)
        {
            StartCoroutine(SwipeCoroutine());
        }
    }
    IEnumerator SwipeCoroutine()
    {
        _allowSwipe = false;
        float eTime = 0;
        while(eTime < _swipeDuration)
        {
            float val = Mathf.Lerp(_maskBoundInit, _maskBoundTarget, eTime / _swipeDuration);
            _spriteRenderer.material.SetFloat("_MaskBound", val);
            yield return null;
            eTime += Time.deltaTime;
        }
    }
}
