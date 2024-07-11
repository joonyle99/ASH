using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 컷씬 플레이어는 다양한 연출을 시퀀스를 통해 재생한다
/// </summary>
public class CutscenePlayer : MonoBehaviour, ITriggerListener
{
    [SerializeField] bool _playOnce = true;
    [SerializeField] bool _played = false;

    [SerializeField] List<SceneEffect> _sequence;

    public bool IsPlaying { get; private set; } = false;
    public bool IsPlayed => _played;

    private PreserveState _statePreserver;

    private void Awake()
    {
        _statePreserver = GetComponent<PreserveState>();

        if (_statePreserver)
        {
            bool played = _statePreserver.LoadState("_played", _played);
            if (played)
            {
                _played = true;
            }
        }
    }
    private void OnDestroy()
    {
        if (_statePreserver)
        {
            _statePreserver.SaveState("_played", _played);
        }
    }

    /// <summary>
    /// 시퀀스에 맞는 연출을 재생한다
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    private IEnumerator PlaySequenceCoroutine(List<SceneEffect> sequence)
    {
        Debug.Log("Sequence 코루틴 시작");

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
                DialogueController.Instance.StartDialogue(effect.DialogueData, true);
                yield return new WaitWhile(() => DialogueController.Instance.IsDialogueActive);
            }
            /*
            else if (effect.Type == SceneEffect.EffectType.LifePurchase)
            {
                GameUIManager.OpenLifePurchasePanel();
                yield return new WaitWhile(() => GameUIManager.IsLifePurchasePanelOpen);
            }
            */
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
                // yield return new WaitWhile(() => effect.Function);
            }
        }

        IsPlaying = false;

        // 플레이어 무적 상태 해제
        SceneContext.Current.Player.IsGodMode = false;
        // Debug.Log($"{SceneContext.Current.Player}의 GodMode가 해제됩니다. => IsGodMode : {SceneContext.Current.Player.IsGodMode}");

        Debug.Log("Sequence 코루틴 종료");
    }

    /// <summary>
    /// 시퀀스를 재생한다
    /// </summary>
    public void Play()
    {
        if (!_played && _playOnce)
        {
            SceneEffectManager.Instance.PushCutscene(new Cutscene(this, PlaySequenceCoroutine(_sequence)));
            _played = true;

            // 플레이어를 무적 상태로 만든다
            SceneContext.Current.Player.IsGodMode = true;
            // Debug.Log($"{SceneContext.Current.Player}의 GodMode가 설정됩니다. => IsGodMode : {SceneContext.Current.Player.IsGodMode}");
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
}
