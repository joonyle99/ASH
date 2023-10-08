using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransitionPlayer : MonoBehaviour, ISceneContextBuildListener
{
    [SerializeField] Image _fadeImage;
    [SerializeField] float _fadeDuration;
    public void PlayExitEffect()
    {
        StartCoroutine(ExitEffectCoroutine());
    }
    public void PlayEnterEffect()
    {
        StartCoroutine(EnterEffectCoroutine());
    }
    public virtual IEnumerator ExitEffectCoroutine()
    {
        yield return FadeCoroutine(_fadeDuration, FadeType.Darken);
    }

    public virtual IEnumerator EnterEffectCoroutine()
    {
        yield return FadeCoroutine(_fadeDuration, FadeType.Lighten);
    }
    public void OnSceneContextBuilt()
    {
        PlayEnterEffect();
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

    //TODO : Global 플레이어 상태 관리 오브젝트로 옮겨야함
    public void ReactivatePlayerAfterDelay(Vector3 spawnPosition, float delay)
    {
        StartCoroutine(ReactivatePlayerAfterDelayCoroutine(spawnPosition, delay));
    }
    IEnumerator ReactivatePlayerAfterDelayCoroutine(Vector3 spawnPosition, float delay)
    {
        yield return FadeCoroutine(_fadeDuration, FadeType.Darken);
        yield return new WaitForSeconds(delay);
        SceneContext.Current.Player.transform.position = spawnPosition;
        SceneContext.Current.Player.gameObject.SetActive(true);
        yield return FadeCoroutine(_fadeDuration, FadeType.Lighten);
    }
}