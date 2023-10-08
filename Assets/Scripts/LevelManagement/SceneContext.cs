using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneContext : MonoBehaviour
{
    public static SceneContext Current { get; private set; }

    public SceneTransitionPlayer SceneTransitionPlayer { get; private set; }
    public PlayerBehaviour Player { get; private set; }
    public List<Passage> Passages { get; private set; } = new List<Passage>();

    private void Awake()
    {
        Current = this;
    }
    public Result BuildPlayable(string entranceName)
    {
        Result buildResult = Result.Success;

        void UpdateBuildResult<T>(T reference)
        {
            if (reference == null)
            {
                Debug.LogWarning("No " + typeof(T).Name + " in scene!");
                buildResult = Result.Fail;
            }
        }

        PlayableSceneTransitionPlayer sceneTransitionPlayer = FindFirstObjectByType<PlayableSceneTransitionPlayer>();
        if (sceneTransitionPlayer != null)
        {
            sceneTransitionPlayer.EntrancePassageName = entranceName;
            SceneTransitionPlayer = sceneTransitionPlayer;
        }
        UpdateBuildResult(sceneTransitionPlayer);

        Player = FindFirstObjectByType<PlayerBehaviour>();
        UpdateBuildResult(Player);

        //Find passages
        Passages = FindObjectsByType<Passage>(FindObjectsSortMode.None).ToList();
        UpdateBuildResult(Passages);

        Result defaultBuildResult = DefaultBuild();
        if (defaultBuildResult == Result.Fail)
            buildResult = Result.Fail;

        return buildResult;
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