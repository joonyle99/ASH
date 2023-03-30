using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneContextController : HappyTools.SingletonBehaviourFixed<SceneContextController>
{
    SceneContext _context;
    public void OnLoad(string entranceName)
    {
        BuildSceneContext();
        StartCoroutine(ExitPassageCoroutine(entranceName));
    }
    void BuildSceneContext()
    {
        _context = new SceneContext();
        _context.Player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        
        var passages = GameObject.FindGameObjectsWithTag("Passage");
        _context.Passages = new List<Passage>();
        for (int i = 0; i < passages.Length; i++)
            _context.Passages.Add( passages[i].GetComponent<Passage>());
    }
    IEnumerator ExitPassageCoroutine(string entranceName)
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