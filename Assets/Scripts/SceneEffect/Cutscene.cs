using System.Collections;
using UnityEngine;

public class Cutscene
{
    private MonoBehaviour _owner;
    public MonoBehaviour Owner => _owner;

    private IEnumerator _cutsceneCoroutine;
    private bool _useLetterbox = true;
    private System.Action _onEndCallback;
    public bool IsDone { get; private set; } = false;
    public bool IsStartted { get; private set; } = false;

    public Coroutine CutSceneCoreCoroutine = null;

    public Cutscene(MonoBehaviour owner, IEnumerator cutsceneCoroutineFunction, bool useLetterbox = true)
    {
        _owner = owner;
        _cutsceneCoroutine = cutsceneCoroutineFunction;
        _useLetterbox = useLetterbox;
    }
    public void Play(System.Action onEndCallback)
    {
        _onEndCallback = onEndCallback;
        _owner.StartCoroutine(CutsceneCoroutine());
    }
    public IEnumerator CutsceneCoroutine()
    {
        // Debug.Log($"{_owner.name}으로부터 시작된 컷씬입니다");

        IsStartted = true;
        IsDone = false;

        if (_useLetterbox)
            GameUIManager.OpenLetterbox();

        yield return CutSceneCoreCoroutine = _owner.StartCoroutine(_cutsceneCoroutine);

        CutSceneCoreCoroutine = null;
        IsDone = true;
        _onEndCallback.Invoke();

        if (_useLetterbox)
            GameUIManager.CloseLetterbox();

        IsStartted = false;

        // Debug.Log($"{_owner.name}으로부터 시작된 컷씬이 종료되었습니다");
    }
}