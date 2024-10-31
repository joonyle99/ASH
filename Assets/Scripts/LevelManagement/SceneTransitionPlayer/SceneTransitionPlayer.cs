using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� ��ȯ�� ��, ȭ���� Fade In / Out �Ǵ� ��ȯ ȿ��
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

        // _fadeImage.color.a 0: ������ �̹����� ����
        // _fadeImage.color.a 1: ������ �̹����� ������

        float from = _fadeImage.color.a;
        float to = 1f;

        // FadeType�� ���� ���� �� ���� ���� ����
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