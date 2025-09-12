using UnityEngine;
using System.Linq;
using Com.LuisPedroFonseca.ProCamera2D;

/// <summary>
/// 현재 씬에 대한 동작은 모두 담당함.
/// 씬의 '주요 오브젝트'에 대한 레퍼런스를 갖고 있음
/// </summary>
public class SceneContext : MonoBehaviour
{
    public static SceneContext Current { get; private set; }                // singleton

    // basic
    public PlayerBehaviour Player { get; private set; }                     // 플레이어
    public ProCamera2D ProCamera { get; private set; }                      // 프로 카메라
    public CameraController CameraController { get; private set; }          // 카메라 컨트롤러

    // extra
    public Passage EntrancePassage { get; private set; }                                                                                // 씬의 입구
    public SceneTransitionPlayer SceneTransitionPlayer { get; private set; }                                                            // 씬 전환 플레이어 (타이틀, 프롤로그로의 이동)
    public PlayableSceneTransitionPlayer PlayableSceneTransitionPlayer => SceneTransitionPlayer as PlayableSceneTransitionPlayer;      // 씬 전환 플레이어 (탐험구간, 보스던전, 보스전으로의 이동)

    [SerializeField] private CheckpointManager _checkpointManager;          // 체크포인트 매니저
    public CheckpointManager CheckPointManager => _checkpointManager;

    protected void Awake()
    {
        Current = this;
        ProCamera = FindObjectOfType<ProCamera2D>();
        CameraController = ProCamera.GetComponent<CameraController>();

        // SceneContext에 체크포인트 매니저를 추가한다
        if (_checkpointManager == null)
        {
            _checkpointManager = GetComponentInChildren<CheckpointManager>();

            if (_checkpointManager == null)
                _checkpointManager = gameObject.AddComponent<CheckpointManager>();
        }
    }

    /// <summary>
    /// 기본적으로 새로운 씬의 Start에서 호출되고, 추가로 NonPlayableScene, PlayableScene으로의 전환 시
    /// 해당 씬에 대한 컨텍스트를 가져온다. i.e) 플레이어, 씬 전환 플레이어, 시작 입구, 체크 포인트, 컨텍스트 리스너 ...
    /// </summary>
    public Result BuildPlayable(string entranceName)
    {
        // Debug.Log($"call build playable in {name}");

        Result buildResult = Result.Success;

        // check build result
        void CheckBuildResult<T>(T target)
        {
            // local function

            if (target == null)
            {
                Debug.LogWarning($"<color=orange>[SceneContext Build]</color> There is No {typeof(T).Name} in scene !");

                if (buildResult == Result.Success)
                    buildResult = Result.Fail;
            }
        }

        // 1. player
        Player = FindFirstObjectByType<PlayerBehaviour>();

        // 2. scene transition player
        var playableTransitions = FindObjectsOfType<PlayableSceneTransitionPlayer>();   // 모든 playable scene transition player를 찾는다
        for (int index = 0; index < playableTransitions.Length; index++)
        {
            // Debug.Log($"Scene Transition Player: {playableTransitions[index].gameObject.name}");

            // scene change manager가 붙어있지 않은 (우선순위가 높은) scene transition player를 찾아서 할당해준다 (ex: Initial Scene Transition Player) 
            if (playableTransitions[index].GetComponent<SceneChangeManager>() == null)
            {
                // Debug.Log($"Scene Transition Player: {playableTransitions[index].gameObject.name}");

                SceneTransitionPlayer = playableTransitions[index];
                break;
            }
        }

        // 3. entrance
        var passages = FindObjectsByType<Passage>(FindObjectsSortMode.None);
        EntrancePassage = passages.ToList().Find(element => element.PassageName == entranceName);

        // check build result for player and entrance
        CheckBuildResult(Player);
        CheckBuildResult(EntrancePassage);

        // build check point
        Result checkpointBuildResult = _checkpointManager.CheckPointBuild();
        if (checkpointBuildResult == Result.Fail)
            buildResult = Result.Fail;

        // build specific
        Result sceneSpecificBuildResult = SceneSpecificBuild();
        if (sceneSpecificBuildResult == Result.Fail)
            buildResult = Result.Fail;

        // ** build default **
        Result defaultBuildResult = DefaultBuild();
        if (defaultBuildResult == Result.Fail)
            buildResult = Result.Fail;

        // Debug.Log($"SceneContext 정상적 빌드 성공 여부: {buildResult == Result.Success}");

        return buildResult;
    }

    /// <summary>
    /// 씬 컨텍스트 빌드 시 기본적으로 호출되는 함수.
    /// 특수한 씬 전환 플레이어가 없다면 기본 씬 전환 플레이어를 찾아서 설정해준다.
    /// 또한 씬이 전환되어 씬 컨텍스트가 새로 빌드되면, 모든 리스너를 찾아 이벤트를 전달한다
    /// i.e) 씬 전환 시, 씬의 입구에서 나오는 컷씬, 플레이어 상태 초기화, 보스 던전 UI 표시 등
    /// </summary>
    public Result DefaultBuild()
    {
        Result buildResult = Result.Success;

        // 기본 씬 전환 플레이어를 설정해준다
        if (SceneTransitionPlayer == null)
        {
            SceneTransitionPlayer = FindFirstObjectByType<SceneTransitionPlayer>();

            if (SceneTransitionPlayer == null)
            {
                // Debug.LogError("There is No Any Scene Transition Player in scene ! (even SceneChangeManager)");
                buildResult = Result.Fail;
            }
        }

        // broadcast to all scene context build listener
        if (buildResult == Result.Success)
        {
            var monoBehaviours = FindObjectsOfType<MonoBehaviour>(true);                // should include inactive objects
            var buildListeners = monoBehaviours.OfType<ISceneContextBuildListener>();

            // 우선순위 별 정렬
            buildListeners = buildListeners.OrderBy(listener =>
            {
                return listener.Priority;
            }).ToArray();
            
            foreach (var buildListener in buildListeners)
            {
                // Debug.Log($"SceneContextBuildListener: {buildListener.GetType()} priority: {buildListener.Priority}");

                // 씬 컨텍스트 빌드 완료 이벤트를 전달한다
                buildListener.OnSceneContextBuilt();
            }
        }

        return buildResult;
    }

    // extra
    protected virtual Result SceneSpecificBuild()
    {
        return Result.Success;
    }
    public void PlayerInstantRespawn()
    {
        PlayableSceneTransitionPlayer.PlayInstantRespawnEffect(_checkpointManager.LatestCheckpointPosition);
    }
}