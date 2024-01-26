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
    Rect _bounds;

    SpriteRenderer _spriteRenderer;
    float _maskBoundInit;
    float _maskBoundTarget;

    bool _allowSwipe = true;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void SetEnumValue(Material material, string keyPrefix, Direction value)
    {
        material.SetFloat(keyPrefix, (int)value);
        foreach(Direction dir in System.Enum.GetValues(typeof(Direction)))
        {
            if (dir == value)
                material.EnableKeyword(keyPrefix + "_" + dir.ToString().ToUpper());
            else
                material.DisableKeyword(keyPrefix + "_" + dir.ToString().ToUpper());
        }
    }
    public void InstantReveal()
    {
        _allowSwipe = false;
        _spriteRenderer.material.SetFloat("_MaskBound", _maskBoundTarget);
    }
    public void InitMask(Direction swipeDirection)
    {
        _swipeDirection = swipeDirection;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        SetEnumValue(_spriteRenderer.material, "_SWIPEDIRECTION", _swipeDirection);

        float gradientSize = _spriteRenderer.material.GetFloat("_GradientSize");
        _bounds = new Rect(transform.position - transform.lossyScale / 2, transform.lossyScale);
        if (_swipeDirection == Direction.Left)
        {
            _maskBoundInit= _bounds.xMax;
            _maskBoundTarget = _bounds.xMin - gradientSize;
        }
        if (_swipeDirection == Direction.Right)
        {
            _maskBoundInit= _bounds.xMin;
            _maskBoundTarget = _bounds.xMax + gradientSize;
        }
        if (_swipeDirection == Direction.Down)
        {
            _maskBoundInit = _bounds.yMax;
            _maskBoundTarget = _bounds.yMin - gradientSize;
        }
        if (_swipeDirection == Direction.Up)
        {
            _maskBoundInit = _bounds.yMin;
            _maskBoundTarget = _bounds.yMax + gradientSize;
        }
        _spriteRenderer.material.SetFloat("_MaskBound", _maskBoundInit);
    }
    public void OnLightCaptured(float swipeDuration)
    {
        if (_allowSwipe)
        {
            StartCoroutine(SwipeCoroutine(swipeDuration));
        }
    }
    IEnumerator SwipeCoroutine(float swipeDuration)
    {
        _allowSwipe = false;
        float eTime = 0;
        while(eTime < swipeDuration)
        {
            float val = Mathf.Lerp(_maskBoundInit, _maskBoundTarget, eTime / swipeDuration);
            _spriteRenderer.material.SetFloat("_MaskBound", val);
            yield return null;
            eTime += Time.deltaTime;
        }
    }
}
