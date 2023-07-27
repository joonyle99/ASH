using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SceneManager : HappyTools.SingletonBehaviourFixed<SceneManager>
{
    [SerializeField] Image _fadeImage;
    [SerializeField] float _fadeDuration;
    SceneContextController _sceneContext;

    [SerializeField] KeyCode _restartKey;

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


    //TEMP
    private void Update()
    {
        if(Input.GetKeyDown(_restartKey))
        {
            StartSceneChange("TitleScene");
        }
    }
    public void StartSceneChange(string sceneName)
    {
        if (_isTransitioning)
            return;
        Instance.StartCoroutine(TransitionCoroutine(sceneName));
    }
    public void StartSceneChangeByPassage(PassageData targetPassageData)
    {
        if (_isTransitioning)
            return;

        Camera.main.GetComponent<CameraController>().DisableCameraFollow();

        Instance.StartCoroutine(TransitionCoroutine(targetPassageData.TargetSceneName, targetPassageData.Name));
    }
    IEnumerator InitialStartCoroutine()
    {
        yield return StartCoroutine(StartSceneAfterLoadCoroutine());
    }
    IEnumerator TransitionCoroutine(string sceneName, string entranceName = "")
    {
        _isTransitioning = true;
        yield return FadeCoroutine(_fadeDuration, 0, 1);
        //TODO : Close scene?
        AsyncOperation load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitUntil(() => load.isDone);

        yield return StartCoroutine(StartSceneAfterLoadCoroutine(entranceName));
    }

    IEnumerator StartSceneAfterLoadCoroutine(string entranceName = "")
    {
        //OnLoad
        Result buildResult = _sceneContext.BuildSceneContext();
        
        if (buildResult == Result.Success)
        {
            _sceneContext.StartEnterPassage(entranceName);
            foreach (ISceneContextBuildListener listener in FindObjectsOfType<MonoBehaviour>().OfType<ISceneContextBuildListener>())
            {
                listener.OnSceneContextBuilt();
            }
        }

        yield return FadeCoroutine(_fadeDuration, 1, 0);
        _isTransitioning = false;
    }

    public void ReactivatePlayerAfterDelay(Vector3 spawnPosition, float delay)
    {
        StartCoroutine(ReactivatePlayerAfterDelayCoroutine(spawnPosition, delay));
    }
    IEnumerator ReactivatePlayerAfterDelayCoroutine(Vector3 spawnPosition, float delay)
    {
        yield return FadeCoroutine(_fadeDuration, 0, 1);
        yield return new WaitForSeconds(delay);
        SceneContextController.Player.transform.position = spawnPosition;
        SceneContextController.Player.gameObject.SetActive(true);
        yield return FadeCoroutine(_fadeDuration, 1, 0);
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
