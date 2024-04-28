using System.Collections.Generic;
using System.Linq;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

/// <summary>
/// 현재 씬에 대한 동작은 모두 담당함.
/// 씬의 주요 오브젝트에 대한 레퍼런스를 갖고 있음
/// </summary>
public class SceneContext : MonoBehaviour
{
    public static SceneContext Current { get; private set; }

    public SceneTransitionPlayer SceneTransitionPlayer { get; private set; }
    public PlayerBehaviour Player { get; private set; }

    public Passage EntrancePassage { get; private set; }

    List<Passage> _passages = new List<Passage>();

    public ProCamera2D Camera { get; private set; }

    [SerializeField] CheckpointManager _checkpointManager;

    public CheckpointManager CheckPointManager { get { return _checkpointManager; } }
    private PlayableSceneTransitionPlayer PlayableSceneTransitionPlayer { get { return SceneTransitionPlayer as PlayableSceneTransitionPlayer; } }
    protected void Awake()
    {
        Current = this;
        if (_checkpointManager == null)
        {
            _checkpointManager = GetComponentInChildren<CheckpointManager>();
            if (_checkpointManager == null)
                _checkpointManager = gameObject.AddComponent<CheckpointManager>();
        }
        Camera = FindObjectOfType<ProCamera2D>();
    }
    public void InstantRespawn()
    {
        PlayableSceneTransitionPlayer.PlayInstantRespawnEffect(_checkpointManager.LatestCheckpointPosition);
    }
    public Result BuildPlayable(string entranceName)
    {
        Result buildResult = Result.Success;

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
        int i = 0;
        while(i < transitionPlayers.Length)
        {
            if (transitionPlayers[i].GetComponent<SceneChangeManager>() == null)
            {
                SceneTransitionPlayer = transitionPlayers[i];
                break;
            }
            i++;
        }
        UpdateBuildResult(SceneTransitionPlayer);


        Player = FindFirstObjectByType<PlayerBehaviour>();
        UpdateBuildResult(Player);

        //Find passages
        _passages = FindObjectsByType<Passage>(FindObjectsSortMode.None).ToList();
        UpdateBuildResult(_passages);

        //Find entrance
        EntrancePassage = _passages.Find(x => x.PassageName == entranceName);
        if (EntrancePassage == null)
        {
            Debug.LogWarning("Passage " + entranceName + " is not found in this scene !!");
            if (_passages.Count > 0)
            {
                EntrancePassage = _passages.Find(x => x.PassageName == "Entrance");
            }

        }

        //Build CheckpointManager
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
    protected virtual Result SceneSpecificBuild()
    {
        return Result.Success;
    }
    Result DefaultBuild()
    {
        Result buildResult = Result.Success;
        //find refs
        if (SceneTransitionPlayer == null)
            SceneTransitionPlayer = FindFirstObjectByType<SceneTransitionPlayer>();

        //broadcast OnLoad()
        if (buildResult == Result.Success)
        {
            foreach (ISceneContextBuildListener listener in FindObjectsOfType<MonoBehaviour>().OfType<ISceneContextBuildListener>())
            {
                listener.OnSceneContextBuilt();
            }
        }
        return buildResult;
    }
}