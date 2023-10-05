using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayableSceneTransitionController : SceneTransitionController
{
    [SerializeField] PlayerBehaviour _player;
    [SerializeField] List<Passage> _passages;
    [SerializeField] float _transitionDuration = 0.5f;
    private void Awake()
    {
        Result buildSuccess = BuildSceneInfo();
        if (buildSuccess == Result.Fail)
            Debug.LogError("Failed to find scene info");
        else
        {
            StartCoroutine(EnterEffectCoroutine());
        }
    }
    public override IEnumerator ExitEffectCoroutine()
    {
        yield return null;
    }

    public override IEnumerator EnterEffectCoroutine()
    {/*
        yield return null;
        Passage entrance = _passages.Find(x => x.Data.Name == entranceName);
        if (entrance == null)
        {
            Debug.LogWarning("Passage " + entranceName + " is not found in this scene !!");
            InputManager.Instance.ChangeToDefaultSetter();
            yield break;
        }
        yield return StartCoroutine(entrance.PlayerExitCoroutine(_context.Player));

        InputManager.Instance.ChangeToDefaultSetter();
        */
        yield return null;

    }

    public Result BuildSceneInfo()
    {
        //Find player
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("No player in scene!");
            return Result.Fail;
        }
        _player = player.GetComponent<PlayerBehaviour>();

        //Find passages
        var passages = GameObject.FindGameObjectsWithTag("Passage");
        _passages = new List<Passage>();
        for (int i = 0; i < passages.Length; i++)
            _passages.Add(passages[i].GetComponent<Passage>());

        return Result.Success;
    }
}