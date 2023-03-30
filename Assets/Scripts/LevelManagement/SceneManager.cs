using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : HappyTools.SingletonBehaviourFixed<SceneManager>
{
    [SerializeField] Image _fadeImage;
    [SerializeField] float _fadeDuration;
    SceneContextController _sceneContext;

    bool _isTransitioning;

    protected override void Awake()
    {
        base.Awake();
        _sceneContext = GetComponent<SceneContextController>();
    }

    private void Start()
    {
        StartCoroutine(InitialStartCoroutine());
    }
    public void StartSceneChangeByPassage(PassageData targetPassageData)
    {
        if (_isTransitioning)
            return;
        Instance.StartCoroutine(TransitionCoroutine(targetPassageData));
    }
    IEnumerator InitialStartCoroutine()
    {
        yield return StartCoroutine(StartSceneAfterLoadCoroutine());
    }
    IEnumerator TransitionCoroutine(PassageData targetPassageData)
    {
        _isTransitioning = true;
        yield return FadeCoroutine(_fadeDuration, 0, 1);
        //TODO : Close scene?
        AsyncOperation load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(targetPassageData.TargetSceneName);
        yield return new WaitUntil(() => load.isDone);
        
        yield return StartCoroutine(StartSceneAfterLoadCoroutine());
    }

    IEnumerator StartSceneAfterLoadCoroutine()
    {
        //OnLoad
        _sceneContext.OnLoad();
        //Find other passage and override inputsetter
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
