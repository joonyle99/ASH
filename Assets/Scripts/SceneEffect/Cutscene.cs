using System.Collections;
using UnityEngine;

public class Cutscene
{
    MonoBehaviour _owner;
    IEnumerator _coroutineFunction;
    bool _useLetterbox = true;
    System.Action _onEndCallback;
    public bool IsDone { get; private set; } = false;
    public bool IsStartted { get; private set; } = false;
    public Cutscene(MonoBehaviour owner, IEnumerator coroutineFunction, bool useLetterbox = true)
    {
        _owner = owner;
        _coroutineFunction = coroutineFunction;
        _useLetterbox = useLetterbox;
    }
    public void Play(System.Action onEndCallback)
    {
        _onEndCallback = onEndCallback;
        _owner.StartCoroutine(CutsceneCoroutine());
    }
    public IEnumerator CutsceneCoroutine()
    {
        IsStartted = true;
        IsDone = false;
        if (_useLetterbox)
            GameUIManager.OpenLetterbox();
        yield return _owner.StartCoroutine(_coroutineFunction);
        IsDone = true;
        _onEndCallback.Invoke();
        if (_useLetterbox)
            GameUIManager.CloseLetterbox();
        IsStartted = false;

        // 플레이어를 무적 상태로 만든다
        SceneContext.Current.Player.IsGodMode = false;
    }
}