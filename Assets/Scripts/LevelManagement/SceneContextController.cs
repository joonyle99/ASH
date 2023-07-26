using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneContextController : HappyTools.SingletonBehaviourFixed<SceneContextController>
{
    [SerializeField]
    public class SceneContext
    {
        public PlayerBehaviour Player;
        public List<Passage> Passages;
    }

    SceneContext _context;

    bool _recentBuildSucceeded = false;
    public static bool RecentBuildSucceeded { get { return Instance._recentBuildSucceeded; } }

    public static PlayerBehaviour Player { get { return Instance._context.Player; } }

    public Result BuildSceneContext()
    {
        _context = new SceneContext();
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("No player in scene!");
            _recentBuildSucceeded = false;
            return Result.Fail;
        }
        _context.Player = player.GetComponent<PlayerBehaviour>();


        var passages = GameObject.FindGameObjectsWithTag("Passage");
        _context.Passages = new List<Passage>();
        for (int i = 0; i < passages.Length; i++)
            _context.Passages.Add( passages[i].GetComponent<Passage>());

        _recentBuildSucceeded = true;
        return Result.Success;
    }
    public void StartEnterPassage(string entranceName)
    {
        StartCoroutine(EnterPassageCoroutine(entranceName));
    }
    IEnumerator EnterPassageCoroutine(string entranceName)
    {
        Passage entrance = _context.Passages.Find(x => x.Data.Name == entranceName);
        if (entrance == null)
        {
            Debug.LogWarning("Passage " + entranceName + " is not found in this scene !!");
            InputManager.Instance.ChangeToDefaultSetter();
            yield break;
        }
        yield return StartCoroutine(entrance.PlayerExitCoroutine(_context.Player));

        InputManager.Instance.ChangeToDefaultSetter();
        yield return null;
    }
}