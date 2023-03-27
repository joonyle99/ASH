using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : HappyTools.SingletonBehaviourFixed<SceneTransitionManager>
{
    [SerializeField] Image _fadeImage;
    [SerializeField] float _fadeDuration;

    bool _isTransitioning;
    public void ChangeScene(string sceneName)
    {
        if (_isTransitioning)
            return;
        Instance.StartCoroutine(TransitionCoroutine(sceneName));
    }
    IEnumerator TransitionCoroutine(string targetSceneName)
    {
        _isTransitioning = true;
        yield return FadeCoroutine(_fadeDuration, 0, 1);
        AsyncOperation load = SceneManager.LoadSceneAsync(targetSceneName);
        yield return new WaitUntil(()=>!load.isDone);
        yield return FadeCoroutine(_fadeDuration, 1, 0);
        _isTransitioning = false;
    }
    IEnumerator FadeCoroutine(float duration, float from, float to)
    {
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
