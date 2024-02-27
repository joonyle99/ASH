using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransitionPlayer : MonoBehaviour
{
    [SerializeField] Image _fadeImage;
    [SerializeField] float _fadeDuration;

    protected float FadeDuration { get { return _fadeDuration; } }

    public virtual IEnumerator ExitEffectCoroutine()
    {
        yield return FadeCoroutine(_fadeDuration, FadeType.Darken);
    }

    public virtual IEnumerator EnterEffectCoroutine()
    {
        yield return FadeCoroutine(_fadeDuration, FadeType.Lighten);
    }


    protected enum FadeType { Lighten, Darken}
    protected IEnumerator FadeCoroutine(float duration, FadeType fadeType)
    {
        float from = fadeType == FadeType.Darken ? 0f : 1f;
        float to = fadeType == FadeType.Darken ? 1f : 0f;

        if(_fadeImage == null)
        {
            Debug.LogWarning("No Fade Image!!");
            yield break;
        }

        float eTime = 0f;
        Color imageColor = _fadeImage.color;
        while (eTime < duration)
        {
            imageColor.a = Mathf.Lerp(from, to, eTime / duration);
            _fadeImage.color = imageColor;
            yield return null;
            eTime += Time.deltaTime;
        }
        imageColor.a = to;
        _fadeImage.color = imageColor;
    }
}