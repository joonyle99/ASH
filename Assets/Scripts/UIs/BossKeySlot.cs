using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class BossKeySlot : MonoBehaviour
{
    [SerializeField] Image _keyImage;

    private Vector2 _originSize;
    private float _originAlpha;

    private Coroutine _obtainCoroutine;

    private void Awake()
    {
        _originSize = _keyImage.rectTransform.sizeDelta;
        _originAlpha = _keyImage.color.a;
    }

    private void InitImage()
    {
        _keyImage.rectTransform.sizeDelta = _originSize;
        var imageColor = _keyImage.color;
        imageColor.a = _originAlpha;
        _keyImage.color = imageColor;
    }

    public void SetValue(bool obtained)
    {
        _keyImage.enabled = obtained;
    }
    public void Obtain()
    {
        SetValue(true);

        if (_obtainCoroutine != null)
        {
            StopCoroutine(_obtainCoroutine);
            _obtainCoroutine = StartCoroutine(ObtainCoroutine());
        }
    }

    private IEnumerator ObtainCoroutine()
    {
        InitImage();

        var originalSize = _keyImage.rectTransform.sizeDelta;
        var startSize = originalSize * 2;

        var color = _keyImage.color;
        float originalAlpha = color.a;

        float eTime = 0f;
        float duration = 0.5f;

        while (eTime < duration)
        {
            color.a = Mathf.Lerp(0, originalAlpha, Curves.EaseIn(eTime / duration));
            _keyImage.color = color;
            _keyImage.rectTransform.sizeDelta = Vector2.Lerp(startSize, originalSize, Curves.EaseOut(eTime / duration));

            yield return null;

            eTime += Time.deltaTime;
        }

        color.a = 1;
        _keyImage.color = color;
        _keyImage.rectTransform.sizeDelta = originalSize;
    }
}
