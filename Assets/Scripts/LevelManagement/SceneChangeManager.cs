using LevelGraph;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[Serializable]
public enum SceneChangeType
{
    None = 0,
    ChangeMap = 1,
    Loading = 2,
    StageReset = 3,
    PlayerRespawn = 4,
}

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

    [SerializeField] private SceneChangeType _sceneChangeType = SceneChangeType.None;
    public SceneChangeType SceneChangeType
    {
        get => _sceneChangeType;
        set => _sceneChangeType = value;
    }

    private void Start()
    {
        var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        _sceneChangeType = SceneChangeType.None;

        if (GameSceneManager.IsDefinedScene(sceneName))
        {
            // create sceneContext
            SceneContext sceneContext = FindOrCreateSceneContext();

            // entrance
            var passages = FindObjectsByType<Passage>(FindObjectsSortMode.None);
            var targetEntranceName = "Enter " + sceneName;
            var hasEntrance = passages.ToList().Find(passage => passage.PassageName == targetEntranceName);
            var entranceName = hasEntrance ? targetEntranceName : "";

            // build sceneContext
            Result buildResult = sceneContext.BuildPlayable(entranceName);
        }

        // play BGM
        SoundManager.Instance.PlayCommonBGMForScene(sceneName);

        // TEMP: set scene name text for debug
        // GameUIManager.SetSceneNameText(sceneName);
    }

    public SceneContext FindOrCreateSceneContext()
    {
        SceneContext sceneContext = FindFirstObjectByType<SceneContext>();

        // SceneContext를 생성해주는 경우
        if (sceneContext == null)
        {
            Debug.Log("SceneContext doesn't exist in the scene, so we'll create a new one (exception: LanternSceneContext)");

            GameObject go = new GameObject("SceneContext (Created)");
            sceneContext = go.AddComponent<SceneContext>();
        }
        // 이미 존재하는 SceneContext가 있는 경우
        else
        {
            // Debug.Log("The scene already has a sceneContext. Returns it immediately");
        }

        return sceneContext;
    }
    public PassageData GetNextPassageData(string fromPassageName)
    {
        var fromSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        var fromPassageData = new PassageData(fromSceneName, fromPassageName);
        var toPassageData = _levelGraphData.GetExitPassageData(fromPassageData);

        if (toPassageData.SceneName == "")
        {
            Debug.LogWarning("To Passage Data has no scene name");
            return new PassageData(fromSceneName, fromPassageName);
        }

        return toPassageData;
    }

    // 플레이 불가능한 씬으로 전환 (타이틀 씬, 프롤로그 씬, 엔딩 씬 등)
    public void ChangeToNonPlayableScene(string sceneName, System.Action changeDoneCallback = null)
    {
        if (IsChanging)
            return;

        StartCoroutine(ChangeToNonPlayableSceneCoroutine(sceneName, changeDoneCallback));
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

        // GameUIManager.SetSceneNameText(sceneName);
    }

    // 플레이 가능한 씬으로 전환 (탐험구간, 보스던전, 보스전 등)
    public void ChangeToPlayableScene(string sceneName, string passageName)
    {
        if (IsChanging)
            return;

        if (SceneChangeType == SceneChangeType.Loading)
        {
            DialogueDataManager.LoadSyncAllDialogueData(true);
        }
        else
        {
            DialogueDataManager.LoadSyncAllDialogueData(false);
        }

        StartCoroutine(ChangeToPlayableSceneCoroutine(sceneName, passageName));
    }
    private IEnumerator ChangeToPlayableSceneCoroutine(string sceneName, string entranceName)
    {
        IsChanging = true;

        AsyncOperation load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitUntil(() => load.isDone);

        if (Time.timeScale != 0)
        {
            Time.timeScale = 1;
        }

        SceneContext sceneContext = FindOrCreateSceneContext();
        Result buildResult = sceneContext.BuildPlayable(entranceName);      // scene context build

        IsChanging = false;

        // 씬에 대한 BGM 재생
        SoundManager.Instance.PlayCommonBGMForScene(sceneName);

        // GameUIManager.SetSceneNameText(sceneName);
    }

    // SceneContext가 DefaultBuild되었을 때 호출되는 함수 (ISceneContextBuildListener 인터페이스 구현)
    public void OnSceneContextBuilt()
    {
        // 플레이어가 씬의 입구에서 나오는 컷씬을 실행한다
        StartCoroutine(SceneEffectManager.Instance.PushCutscene(new Cutscene(this, SceneContext.Current.SceneTransitionPlayer.EnterSceneEffectCoroutine(), false)));

        if (SceneContext.Current.SceneTransitionPlayer != _defaultSceneTransitionPlayer)
        {
            // GameUIManager.SetDebugText("SetFadeImageAlpha(0f)");
            _defaultSceneTransitionPlayer.SetFadeImageAlpha(0f);
        }
    }
}