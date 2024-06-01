using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 씬이 전환될 때, 화면이 Fade In / Out 되는 전환 효과
/// </summary>
public class SceneTransitionPlayer : MonoBehaviour
{
    protected enum FadeType { Lighten, Darken }

    [SerializeField] Image _fadeImage;
    [SerializeField] float _fadeDuration;

    protected float FadeDuration => _fadeDuration;

    public void SetFadeImageAlpha(float alpha)
    {
        Color color = _fadeImage.color;
        color.a = alpha;
        _fadeImage.color = color;
    }

    public virtual IEnumerator EnterSceneEffectCoroutine()
    {
        yield return FadeCoroutine(_fadeDuration, FadeType.Lighten);
    }
    public virtual IEnumerator ExitSceneEffectCoroutine()
    {
        yield return FadeCoroutine(_fadeDuration, FadeType.Darken);
    }
    protected IEnumerator FadeCoroutine(float duration, FadeType fadeType)
    {
        float from = fadeType == FadeType.Darken ? 0f : 1f;
        float to = fadeType == FadeType.Darken ? 1f : 0f;

        if (_fadeImage == null)
        {
            Debug.LogWarning("No Fade Image!!");
            yield break;
        }

        Color imageColor = _fadeImage.color;

        float eTime = 0f;
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