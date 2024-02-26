using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Letterbox : MonoBehaviour
{
    [SerializeField] float _letterboxOpenDuration = 1f;
    [SerializeField] float _letterboxCloseDuration = 0.5f;
    [SerializeField] float _letterboxHeight = 100f;

    [SerializeField] Image _topImage;
    [SerializeField] Image _bottomImage;

    private void Awake()
    {
        SetHeight(0);
    }

    public void Open(bool instant = false)
    {
        StartCoroutine(OpenCoroutine(instant));
    }
    IEnumerator OpenCoroutine(bool instant)
    {
        float eTime = 0f;
        Color color = _topImage.color;
        if (!instant)
        {
            while (eTime < _letterboxOpenDuration)
            {
                float t = (eTime / _letterboxOpenDuration);
                color.a = t;
                _topImage.color = color;
                _bottomImage.color = color;

                float height = Mathf.Lerp(0, _letterboxHeight, t);
                SetHeight(height);
                yield return null;
                eTime += Time.deltaTime;
            }
        }
        color.a = 1;
        _topImage.color = color;
        _bottomImage.color = color;
        SetHeight(_letterboxHeight);
    }
    void SetHeight(float height)
    {
        _topImage.rectTransform.sizeDelta = new Vector2(_topImage.rectTransform.sizeDelta.x, height);
        _bottomImage.rectTransform.sizeDelta = new Vector2(_bottomImage.rectTransform.sizeDelta.x, height);
    }
    public void Close()
    {
        StartCoroutine(CloseCoroutine());
    }
    IEnumerator CloseCoroutine()
    {
        float eTime = 0f;
        Color color = _topImage.color;
        while (eTime < _letterboxCloseDuration)
        {
            float t = (eTime / _letterboxCloseDuration);
            color.a = 1 - t;
            _topImage.color = color;
            _bottomImage.color = color;

            float height = Mathf.Lerp(_letterboxHeight, 0, t);
            SetHeight(height);
            yield return null;
            eTime += Time.deltaTime;
        }
        color.a = 0;
        _topImage.color = color;
        _bottomImage.color = color;
        SetHeight(0);
    }
}
