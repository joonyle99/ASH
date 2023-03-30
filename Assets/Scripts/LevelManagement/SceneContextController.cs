using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneContextController : HappyTools.SingletonBehaviourFixed<SceneContextController>
{
    SceneContext _context;
    public void OnLoad()
    {
        Debug.Log("ONLOAD");
        BuildSceneContext();
        StartCoroutine(ExitPassageCoroutine());
    }
    void BuildSceneContext()
    {
        _context = new SceneContext();
        _context.Player = GameObject.FindWithTag("Player").GetComponent<PlayerDummy>();
        
        var passages = GameObject.FindGameObjectsWithTag("Passage");
        _context.Passages = new Passage[passages.Length];
        for (int i = 0; i < passages.Length; i++)
            _context.Passages[i] = passages[i].GetComponent<Passage>();
    }
    IEnumerator ExitPassageCoroutine()
    {
        Passage passage = FindObjectOfType<Passage>();

        yield return StartCoroutine(passage.PlayerExitCoroutine(_context.Player));

        InputManager.Instance.ChangeToDefaultSetter();
        yield return null;
    }
}