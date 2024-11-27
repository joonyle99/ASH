using System.Collections;
using UnityEngine;

[System.Serializable]
public class Cutscene
{
    private MonoBehaviour _owner;
    public MonoBehaviour Owner => _owner;

    private IEnumerator _cutsceneCoroutine;
    private bool _useLetterbox = true;
    private System.Action _onEndCallback;
    private System.Action _onAdditionalBefore;
    private System.Action _onAdditionalAfter;
    public bool IsDone { get; private set; } = false;
    public bool IsStartted { get; private set; } = false;

    public Coroutine CutSceneCoreCoroutine = null;

    public Cutscene(MonoBehaviour owner, IEnumerator cutsceneCoroutineFunction, bool useLetterbox = true)
    {
        _owner = owner;
        _cutsceneCoroutine = cutsceneCoroutineFunction;
        _useLetterbox = useLetterbox;
    }
    public void Play(System.Action onEndCallback, System.Action onAdditionalBefore = null, System.Action onAdditionalAfter = null)
    {
        _onEndCallback = onEndCallback;
        _onAdditionalBefore = onAdditionalBefore;
        _onAdditionalAfter = onAdditionalAfter;
        _owner.StartCoroutine(CutsceneCoroutine());
    }
    public IEnumerator CutsceneCoroutine()
    {
        // Debug.Log($"{_owner.name}으로부터 시작된 컷씬입니다");

        // ex) ChasingCamera - StopChasing()
        _onAdditionalBefore?.Invoke();

        if (SceneContext.Current != null && SceneContext.Current.Player != null)
            SceneContext.Current.Player.IsGodMode = true;

        // process
        IsStartted = true;
        IsDone = false;

        // letter box
        if (_useLetterbox)
        {
            GameUIManager.OpenLetterbox();
        }

        yield return CutSceneCoreCoroutine = _owner.StartCoroutine(_cutsceneCoroutine);
        CutSceneCoreCoroutine = null;

        // letter box
        if (_useLetterbox)
        {
            GameUIManager.CloseLetterbox();
        }

        // process
        IsStartted = false;
        IsDone = true;

        if (SceneContext.Current != null && SceneContext.Current.Player != null)
            SceneContext.Current.Player.IsGodMode = false;

        // ex) ChasingCamera - StartChasing
        _onAdditionalAfter?.Invoke();

        // Debug.Log($"{_owner.name}으로부터 시작된 컷씬이 종료되었습니다");

        // main end callback
        _onEndCallback?.Invoke();
    }

    public string GetCutsceneName()
    {
        return _owner.name;
    }
}