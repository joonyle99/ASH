using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;

/// <summary>
/// 현재 씬에 대한 동작은 모두 담당함.
/// 씬의 주요 오브젝트에 대한 레퍼런스를 갖고 있음
/// </summary>
public class SceneContext : MonoBehaviour
{
    public static SceneContext Current { get; private set; }        // singleton

    // basic
    public PlayerBehaviour Player { get; private set; }
    public ProCamera2D Camera { get; private set; }

    // extra
    public Passage EntrancePassage { get; private set; }
    public SceneTransitionPlayer SceneTransitionPlayer { get; private set; }
    private PlayableSceneTransitionPlayer PlayableSceneTransitionPlayer => SceneTransitionPlayer as PlayableSceneTransitionPlayer;

    private List<Passage> _passages = new();

    [SerializeField] private CheckpointManager _checkpointManager;
    public CheckpointManager CheckPointManager => _checkpointManager;

    protected void Awake()
    {
        Current = this;
        Camera = FindObjectOfType<ProCamera2D>();

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
        void UpdateBuildResult<T>(T reference)
        {
            if (reference == null)
            {
                Debug.LogWarning("No " + typeof(T).Name + " in scene!");

                if(buildResult == Result.Success)
                    buildResult = Result.Fail;
            }
        }

        var transitionPlayers = FindObjectsOfType<PlayableSceneTransitionPlayer>();

        var count = 0;
        while(count < transitionPlayers.Length)
        {
            if (transitionPlayers[count].GetComponent<SceneChangeManager>() == null)
            {
                SceneTransitionPlayer = transitionPlayers[count];
                break;
            }
            count++;
        }

        // Find
        Player = FindFirstObjectByType<PlayerBehaviour>();
        _passages = FindObjectsByType<Passage>(FindObjectsSortMode.None).ToList();
        EntrancePassage = _passages.Find(x => x.PassageName == entranceName);

        if (EntrancePassage == null)
        {
            Debug.LogWarning("Passage " + entranceName + " is not found in this scene !!");

            if (_passages.Count > 0)
            {
                EntrancePassage = _passages.Find(x => x.PassageName == "Entrance");
            }
        }

        // update
        UpdateBuildResult(Player);
        UpdateBuildResult(_passages);
        UpdateBuildResult(SceneTransitionPlayer);

        // Build CheckpointManager
        Result checkpointBuildResult = _checkpointManager.BuildPlayable();
        if (checkpointBuildResult == Result.Fail)
            buildResult = Result.Fail;

        Result sceneSpecificBuildResult = SceneSpecificBuild();
        if (sceneSpecificBuildResult == Result.Fail)
            buildResult = Result.Fail;

        Result defaultBuildResult = DefaultBuild();
        if (defaultBuildResult == Result.Fail)
            buildResult = Result.Fail;

        return buildResult;
    }
    public Result DefaultBuild()
    {
        Result buildResult = Result.Success;

        // find refs
        if (SceneTransitionPlayer == null)
            SceneTransitionPlayer = FindFirstObjectByType<SceneTransitionPlayer>();

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