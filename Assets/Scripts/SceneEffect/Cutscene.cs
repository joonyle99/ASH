using System.Collections;
using UnityEngine;

public class Cutscene
{
    MonoBehaviour _owner;
    IEnumerator _coroutineFunction;
    System.Action _onEndCallback;
    public Cutscene(IEnumerator coroutineFunction)
    {
        _coroutineFunction = coroutineFunction;
    }
    public void Play(SceneEffectManager sceneEffectManager, System.Action onEndCallback)
    {
        _onEndCallback = onEndCallback;
        sceneEffectManager.StartCoroutine(CutsceneCoroutine(sceneEffectManager));
    }
    public IEnumerator CutsceneCoroutine(SceneEffectManager sceneEffectManager)
    {
        yield return sceneEffectManager.StartCoroutine(_coroutineFunction);
        _onEndCallback.Invoke();
    }
}