using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 씬이 전환될 때, 화면이 Fade In / Out 되는 전환 효과
/// </summary>
public class SceneTransitionPlayer : MonoBehaviour
{
    public enum FadeType
    {
        Lighten,        // fade in
        Darken,         // fade out

        Dim,
    }

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
    public IEnumerator FadeCoroutine(float duration, FadeType fadeType)
    {
        if (_fadeImage == null)
        {
            Debug.LogWarning("No Fade Image!!");
            yield break;
        }

        GameUIManager.SetDebugText($"fadeType: {fadeType.ToString()}");

        // _fadeImage.color.a 0: 검정색 이미지가 투명
        // _fadeImage.color.a 1: 검정색 이미지가 불투명

        float from = _fadeImage.color.a;
        float to = 1f;

        // FadeType에 따라 시작 및 종료 값을 설정
        switch (fadeType)
        {
            case FadeType.Darken:
                to = 1f;
                break;
            case FadeType.Lighten:
                to = 0f;
                break;
            case FadeType.Dim:
                to = 0.2f;
                break;
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