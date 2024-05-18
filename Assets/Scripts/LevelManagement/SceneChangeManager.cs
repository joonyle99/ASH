using LevelGraph;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : HappyTools.SingletonBehaviour<SceneChangeManager>, ISceneContextBuildListener
{
    public bool IsChanging { get; private set; } = false;

    // TEMP : 게임 글로벌 데이터를 저장하는 곳으로 이동
    [SerializeField] private LevelGraphData _levelGraphData;
    [SerializeField] private SceneTransitionPlayer _defaultSceneTransitionPlayer;

    private void Start()
    {
        SceneContext sceneContext = FindOrCreateSceneContext();
        Result buildResult = sceneContext.BuildPlayable("");
    }

    public SceneContext FindOrCreateSceneContext()
    {
        SceneContext sceneContext = FindFirstObjectByType<SceneContext>();

        if (sceneContext == null)
        {
            GameObject go = new GameObject("SceneContext (Created)");
            sceneContext = go.AddComponent<SceneContext>();
        }
        return sceneContext;
    }
    public PassageData GetNextPassageData(string passageName)
    {
        string sceneName = SceneManager.GetActiveScene().name;

        Debug.Log(sceneName);

        var result = _levelGraphData.GetExitPassageData(new PassageData(sceneName, passageName));
        if (result.SceneName == "")
            return new PassageData(sceneName, passageName);
        return result;
    }

    public void ChangeToPlayableScene(string sceneName, string passageName)
    {
        if (IsChanging)
            return;

        StartCoroutine(ChangeToPlayableSceneCoroutine(sceneName, passageName));
    }
    public void ChangeToScene(string sceneName, System.Action changeDoneCallback)
    {
        StartCoroutine(ChangeToSceneCoroutine(sceneName, changeDoneCallback));
    }
    public void ChangeToScene(string sceneName)
    {
        StartCoroutine(ChangeToSceneCoroutine(sceneName, null));
    }

    private IEnumerator ChangeToSceneCoroutine(string sceneName, System.Action changeDoneCallback)
    {
        IsChanging = true;
        AsyncOperation load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitUntil(() => load.isDone);

        SceneContext sceneContext = FindOrCreateSceneContext();
        Result buildResult = sceneContext.BuildPlayable("");
        IsChanging = false;

        if (changeDoneCallback != null)
            changeDoneCallback.Invoke();
    }
    private IEnumerator ChangeToPlayableSceneCoroutine(string sceneName, string passageName)
    {
        IsChanging = true;
        AsyncOperation load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitUntil(() => load.isDone);

        SceneContext sceneContext= FindOrCreateSceneContext();
        Result buildResult = sceneContext.BuildPlayable(passageName);
        IsChanging = false;
    }

    public void OnSceneContextBuilt()
    {
            SceneEffectManager.Current.PushCutscene(new Cutscene(this, SceneContext.Current.SceneTransitionPlayer.EnterSceneEffectCoroutine(), false));

        if (SceneContext.Current.SceneTransitionPlayer != _defaultSceneTransitionPlayer)
            _defaultSceneTransitionPlayer.SetFadeImageAlpha(0);
    }
}
