using UnityEngine;
using System.Linq;
using Com.LuisPedroFonseca.ProCamera2D;

/// <summary>
/// ���� ���� ���� ������ ��� �����.
/// ���� '�ֿ� ������Ʈ'�� ���� ���۷����� ���� ����
/// </summary>
public class SceneContext : MonoBehaviour
{
    public static SceneContext Current { get; private set; }        // singleton

    // basic
    public PlayerBehaviour Player { get; private set; }
    public ProCamera2D Camera { get; private set; }

    // extra
    public Passage EntrancePassage { get; private set; }                                                                                // ���� �Ա�
    public SceneTransitionPlayer SceneTransitionPlayer { get; private set; }
    private PlayableSceneTransitionPlayer PlayableSceneTransitionPlayer => SceneTransitionPlayer as PlayableSceneTransitionPlayer;

    [SerializeField] private CheckpointManager _checkpointManager;          // üũ����Ʈ �Ŵ���
    public CheckpointManager CheckPointManager => _checkpointManager;

    protected void Awake()
    {
        Current = this;
        Camera = FindObjectOfType<ProCamera2D>();

        // SceneContext�� üũ����Ʈ �Ŵ����� �߰��Ѵ�
        if (_checkpointManager == null)
        {
            _checkpointManager = GetComponentInChildren<CheckpointManager>();

            if (_checkpointManager == null)
                _checkpointManager = gameObject.AddComponent<CheckpointManager>();
        }
    }

    // basic
    public Result BuildPlayable(string entranceName)
    {
        Result buildResult = Result.Success;

        // local function
        void CheckBuildResult<T>(T target)
        {
            if (target == null)
            {
                Debug.LogWarning($"There is No {typeof(T).Name} in scene !");

                if (buildResult == Result.Success)
                    buildResult = Result.Fail;
            }
        }

        // find references

        // 1. player
        Player = FindFirstObjectByType<PlayerBehaviour>();

        // 2. scene transition player
        var playableTransitions = FindObjectsOfType<PlayableSceneTransitionPlayer>();
        var count = 0;
        while (count < playableTransitions.Length)
        {
            // scene change manager�� �ƴ� scene transition player�� ã�´�. (ex: Initial Scene Transition Player) 
            if (playableTransitions[count].GetComponent<SceneChangeManager>() == null)
            {
                SceneTransitionPlayer = playableTransitions[count];
                Debug.Log($"Scene Transition Player: {SceneTransitionPlayer.gameObject.name}");
                break;
            }
            count++;
        }

        // 3. entrance
        var passages = FindObjectsByType<Passage>(FindObjectsSortMode.None);
        EntrancePassage = passages.ToList().Find(x => x.PassageName == entranceName);

        // update
        CheckBuildResult(Player);
        CheckBuildResult(EntrancePassage);

        // build
        Result checkpointBuildResult = _checkpointManager.BuildPlayable();
        if (checkpointBuildResult == Result.Fail)
            buildResult = Result.Fail;

        Result sceneSpecificBuildResult = SceneSpecificBuild();
        if (sceneSpecificBuildResult == Result.Fail)
            buildResult = Result.Fail;

        Result defaultBuildResult = DefaultBuild();
        if (defaultBuildResult == Result.Fail)
            buildResult = Result.Fail;

        Debug.Log($"SceneContext ������ ���� ���� ����: {buildResult == Result.Success}");

        return buildResult;
    }
    public Result DefaultBuild()
    {
        Result buildResult = Result.Success;

        if (SceneTransitionPlayer == null)
        {
            SceneTransitionPlayer = FindFirstObjectByType<SceneTransitionPlayer>();

            if (SceneTransitionPlayer == null)
            {
                Debug.LogError("There is No Any Scene Transition Player in scene ! (even SceneChangeManager)");
                buildResult = Result.Fail;
            }
        }

        // broadcast OnLoad()
        if (buildResult == Result.Success)
        {
            foreach (ISceneContextBuildListener listener in FindObjectsOfType<MonoBehaviour>().OfType<ISceneContextBuildListener>())
            {
                listener.OnSceneContextBuilt();
            }
        }

        return buildResult;
    }

    // extra
    protected virtual Result SceneSpecificBuild()
    {
        return Result.Success;
    }
    public void InstantRespawn()
    {
        PlayableSceneTransitionPlayer.PlayInstantRespawnEffect(_checkpointManager.LatestCheckpointPosition);
    }
}