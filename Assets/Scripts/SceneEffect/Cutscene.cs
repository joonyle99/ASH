using System.Collections;
using UnityEngine;

public class Cutscene
{
    MonoBehaviour _owner;
    IEnumerator _coroutineFunction;
    System.Action _onEndCallback;
    public bool IsDone { get; private set; } = false;
    public bool IsStartted { get; private set; } = false;
    public Cutscene(MonoBehaviour owner, IEnumerator coroutineFunction)
    {
        _owner = owner;
        _coroutineFunction = coroutineFunction;
    }
    public void Play(System.Action onEndCallback)
    {
        _onEndCallback = onEndCallback;
        _owner.StartCoroutine(CutsceneCoroutine());
    }
    public IEnumerator CutsceneCoroutine()
    {
        IsStartted = true;
        yield return _owner.StartCoroutine(_coroutineFunction);
        IsDone = true;
        _onEndCallback.Invoke();
    }
}