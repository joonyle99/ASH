using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 컷씬 플레이어는 다양한 연출을 시퀀스를 통해 재생한다
/// </summary>
public class CutscenePlayer : MonoBehaviour, ITriggerListener, ISceneContextBuildListener
{
    [Tooltip("저장 및 불러오기를 제외하고 체크 시 해당 컷씬 단 한번만 재생")]
    [SerializeField] bool _playOnce = true;
    public bool PlayOnce => _playOnce;

    [SerializeField] bool _played = false;
    public bool IsPlayed
    {
        get => _played;
        set { _played = value; }
    }

    [SerializeField] List<SceneEffect> _sequence;

    public bool IsPlaying { get; private set; } = false;

    private PreserveState _statePreserver;

    private void Awake()
    {
        _statePreserver = GetComponent<PreserveState>();
    }

    public void OnSceneContextBuilt()
    {
        if (_statePreserver)
        {
            if (SceneChangeManager.Instance &&
                SceneChangeManager.Instance.SceneChangeType == SceneChangeType.Loading ||
                SceneChangeManager.Instance.SceneChangeType == SceneChangeType.PlayerRespawn)
            {
                bool played = _statePreserver.LoadState("_playSaved", _played);
                if (played)
                {
                    _played = true;
                }
            }
            else
            {
                bool played = _statePreserver.LoadState("_played", _played);
                if (played)
                {
                    _played = true;
                }
            }
        }

        SaveAndLoader.OnSaveStarted += SavePlayedState;
    }

    private void OnDestroy()
    {
        if (_statePreserver)
        {
            _statePreserver.SaveState("_played", _played);
        }

        SaveAndLoader.OnSaveStarted -= SavePlayedState;
    }

    /// <summary>
    /// 시퀀스에 맞는 연출을 재생한다
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    private IEnumerator PlaySequenceCoroutine(List<SceneEffect> sequence)
    {
        IsPlaying = true;

        for (int i = 0; i < sequence.Count; i++)
        {
            var effect = sequence[i];

            if (effect.Type == SceneEffect.EffectType.CameraShake)
            {
                SceneEffectManager.Instance.Camera.StartShake(effect.ShakeData);
            }
            else if (effect.Type == SceneEffect.EffectType.ConstantCameraShake)
            {
                SceneEffectManager.Instance.Camera.StartConstantShake(effect.ConstantShakeData);
            }
            else if (effect.Type == SceneEffect.EffectType.StopConstantCameraShake)
            {
                SceneEffectManager.Instance.Camera.StopConstantShake(effect.Time);
            }
            else if (effect.Type == SceneEffect.EffectType.Dialogue)
            {
                DialogueController.Instance.StartDialogue(effect.DialogueData, false);
                yield return new WaitWhile(() => DialogueController.Instance.IsDialogueActive);
            }
            else if (effect.Type == SceneEffect.EffectType.WaitForSeconds)
            {
                yield return new WaitForSeconds(effect.Time);
            }
            else if (effect.Type == SceneEffect.EffectType.ChangeInputSetter)
            {
                InputManager.Instance.ChangeInputSetter(effect.InputSetter);
            }
            else if (effect.Type == SceneEffect.EffectType.ChangeToDefaultInputSetter)
            {
                InputManager.Instance.ChangeToDefaultSetter();
            }
            else if (effect.Type == SceneEffect.EffectType.FunctionCall)
            {
                effect.Function?.Invoke();
            }
            else if (effect.Type == SceneEffect.EffectType.CoroutineCall)
            {
                var monoBehaviour = effect.TargetScript;
                if (monoBehaviour != null)
                {
                    var methodName = effect.MethodName;

                    // 리플렉션으로 메서드 가져오기
                    var method = monoBehaviour.GetType().GetMethod(methodName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    if (method != null)
                    {
                        // 메서드가 코루틴(즉, IEnumerator 반환)을 실행하는지 확인
                        var result = method.Invoke(monoBehaviour, null); // 매개변수가 없는 메서드 호출
                        if (result is IEnumerator enumerator)
                        {
                            yield return monoBehaviour.StartCoroutine(enumerator);
                        }
                        else
                        {
                            Debug.LogError($"Method '{methodName}' is not a coroutine or does not return IEnumerator.");
                        }
                    }
                    else
                    {
                        Debug.LogError($"Method '{methodName}' not found in {monoBehaviour.GetType().Name}.");
                    }
                }
                else
                {
                    Debug.LogError("TargetScript is invalid.");
                }
            }
        }

        IsPlaying = false;
    }

    /// <summary>
    /// 시퀀스를 재생한다
    /// </summary>
    public void Play()
    {
        if (!_playOnce || !_played)
        {
            _played = true;

            //Debug.Log("Play" + gameObject.name+ "'s cutscene");
            StartCoroutine(SceneEffectManager.Instance.PushCutscene(new Cutscene(this, PlaySequenceCoroutine(_sequence))));
        }
        else
        {
            // InputManager.Instance.ChangeToDefaultSetter();
        }
    }

    /// <summary>
    /// 트리거로 인한 시퀀스 재생
    /// </summary>
    /// <param name="activator"></param>
    /// <param name="reporter"></param>
    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter)
    {
        if (activator.Type == ActivatorType.Player)
            Play();
    }

    private IEnumerator CoroutineFunction()
    {
        yield return null;
    }

    private void SavePlayedState()
    {
        if (_statePreserver)
        {
            _statePreserver.SaveState("_playSaved", _played);
        }
    }
}
