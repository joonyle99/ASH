using LevelGraph;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬 전환을 담당하는 매니저
/// SceneChangeManager GameObject
///     -> SceneChangeManager
///     -> SceneTransitionPlayer
///     -> PlayerableSceneTransitionPlayer
/// </summary>
public class SceneChangeManager : HappyTools.SingletonBehaviourFixed<SceneChangeManager>, ISceneContextBuildListener
{
    public bool IsChanging { get; private set; }

    [SerializeField] private LevelGraphData _levelGraphData;                        // 레벨 그래프
    [SerializeField] private SceneTransitionPlayer _defaultSceneTransitionPlayer;   // 기본적으로 사용할 씬 전환 플레이어 (Either Playable or None Playable)

    private void Start()
    {
        // 씬 컨텍스트 생성
        SceneContext sceneContext = FindOrCreateSceneContext();

        var passagesInCurrentScene = FindObjectsByType<Passage>(FindObjectsSortMode.None);
        var sceneName = SceneManager.GetActiveScene().name;
        var firstEntranceName = "Enter " + sceneName;
        var hasEntrance = passagesInCurrentScene.ToList().Find(passage => passage.PassageName == firstEntranceName);
        var entranceName = hasEntrance ? firstEntranceName : "";

        // 씬 컨텍스트 빌드
        Result buildResult = sceneContext.BuildPlayable(entranceName);

        // 씬에 대한 BGM 재생
        SoundManager.Instance.PlayCommonBGMForScene(sceneName);
    }

    public SceneContext FindOrCreateSceneContext()
    {
        SceneContext sceneContext = FindFirstObjectByType<SceneContext>();

        // SceneContext를 생성해주는 경우
        if (sceneContext == null)
        {
            GameObject go = new GameObject("SceneContext (Created)");
            sceneContext = go.AddComponent<SceneContext>();
        }

        return sceneContext;
    }
    public PassageData GetNextPassageData(string fromPassageName)
    {
        var fromSceneName = SceneManager.GetActiveScene().name;

        var fromPassageData = new PassageData(fromSceneName, fromPassageName);
        var toPassageData = _levelGraphData.GetExitPassageData(fromPassageData);

        if (toPassageData.SceneName == "")
        {
            Debug.LogWarning("To Passage Data has no scene name");
            return new PassageData(fromSceneName, fromPassageName);
        }

        return toPassageData;
    }

    public void ChangeToNonPlayableScene(string sceneName, System.Action changeDoneCallback = null)
    {
        StartCoroutine(ChangeToNonPlayableSceneCoroutine(sceneName, changeDoneCallback));
    }
    public void ChangeToPlayableScene(string sceneName, string passageName)
    {
        if (IsChanging)
            return;

        StartCoroutine(ChangeToPlayableSceneCoroutine(sceneName, passageName));
    }

    private IEnumerator ChangeToNonPlayableSceneCoroutine(string sceneName, System.Action changeDoneCallback)
    {
        IsChanging = true;
        AsyncOperation load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitUntil(() => load.isDone);

        SceneContext sceneContext = FindOrCreateSceneContext();
        Result buildResult = sceneContext.BuildPlayable("");
        IsChanging = false;

        changeDoneCallback?.Invoke();

        // 씬에 대한 BGM 재생
        SoundManager.Instance.PlayCommonBGMForScene(sceneName);
    }
    private IEnumerator ChangeToPlayableSceneCoroutine(string sceneName, string passageName)
    {
        IsChanging = true;
        AsyncOperation load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitUntil(() => load.isDone);

        SceneContext sceneContext = FindOrCreateSceneContext();
        Result buildResult = sceneContext.BuildPlayable(passageName);
        IsChanging = false;

        // 씬에 대한 BGM 재생
        SoundManager.Instance.PlayCommonBGMForScene(sceneName);
    }

    public void OnSceneContextBuilt()
    {
        SceneEffectManager.Instance.PushCutscene(new Cutscene(this, SceneContext.Current.SceneTransitionPlayer.EnterSceneEffectCoroutine(), false));

        if (SceneContext.Current.SceneTransitionPlayer != _defaultSceneTransitionPlayer)
            _defaultSceneTransitionPlayer.SetFadeImageAlpha(0);
    }
}