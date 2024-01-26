using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class BossKeySlot : MonoBehaviour
{
    [SerializeField] Image _keyImage;

    public void SetValue(bool obtained)
    {
        _keyImage.enabled = obtained;
    }
    public void Obtain()
    {
        SetValue(false);
        StartCoroutine(ObtainCoroutine());
    }
    IEnumerator ObtainCoroutine()
    {
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
