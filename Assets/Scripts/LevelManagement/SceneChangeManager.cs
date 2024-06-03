using LevelGraph;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬 전환을 담당하는 매니저
/// </summary>
public class SceneChangeManager : HappyTools.SingletonBehaviourFixed<SceneChangeManager>, ISceneContextBuildListener
{
    public bool IsChanging { get; private set; }

    [SerializeField] private LevelGraphData _levelGraphData;                        // 레벨 그래프
    [SerializeField] private SceneTransitionPlayer _defaultSceneTransitionPlayer;   // 기본적으로 사용할 씬 전환 플레이어 (Either Playable or None Playable)

    private void Start()
    {
        SceneContext sceneContext = FindOrCreateSceneContext();                     // 씬 컨텍스트 생성

        var passagesInCurrentScene = FindObjectsByType<Passage>(FindObjectsSortMode.None);
        var firstEntranceName = "Enter " + SceneManager.GetActiveScene().name;
        var hasEntrance = passagesInCurrentScene.ToList().Find(passage => passage.PassageName == firstEntranceName);
        var entranceName = hasEntrance ? firstEntranceName : "";

        Result buildResult = sceneContext.BuildPlayable(entranceName);              // 씬 컨텍스트 빌드
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
    }
    private IEnumerator ChangeToPlayableSceneCoroutine(string sceneName, string passageName)
    {
        IsChanging = true;
        AsyncOperation load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitUntil(() => load.isDone);

        SceneContext sceneContext = FindOrCreateSceneContext();
        Result buildResult = sceneContext.BuildPlayable(passageName);
        IsChanging = false;

        // 현재 씬에 해당하는 BGM 출력

        // 탐험 구간 1-1 ~ 1-3은 Exploration1
        // 보스 던전 1-1 ~ 1-4는 BoseDungeon1

        // 탐험 구간 2-1 ~ 2-3은 Exploration2
        // 보스 던전 2-1 ~ 2-4는 BoseDungeon2

        switch (sceneName)
        {
            case "1-1":
            case "1-2":
            case "1-3":
                SoundManager.Instance.PlayCommonBGM("Exploration1");
                break;
            case "Boss_1-1":
            case "Boss_1-2":
            case "Boss_1-3":
            case "Boss_1-4":
                SoundManager.Instance.PlayCommonBGM("BoseDungeon1");
                break;
        }
    }

    public void OnSceneContextBuilt()
    {
        SceneEffectManager.Instance.PushCutscene(new Cutscene(this, SceneContext.Current.SceneTransitionPlayer.EnterSceneEffectCoroutine(), false));

        if (SceneContext.Current.SceneTransitionPlayer != _defaultSceneTransitionPlayer)
            _defaultSceneTransitionPlayer.SetFadeImageAlpha(0);
    }
}
