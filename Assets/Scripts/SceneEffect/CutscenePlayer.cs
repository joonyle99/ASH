using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ƾ� �÷��̾�� �پ��� ������ �������� ���� ����Ѵ�
/// </summary>
public class CutscenePlayer : MonoBehaviour, ITriggerListener, ISceneContextBuildListener
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
    }

    public void OnSceneContextBuilt()
    {
        if (_statePreserver)
        {
            if (SceneChangeManager.Instance &&
                SceneChangeManager.Instance.SceneChangeType == SceneChangeType.Loading)
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
    /// �������� �´� ������ ����Ѵ�
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    private IEnumerator PlaySequenceCoroutine(List<SceneEffect> sequence)
    {
        // Debug.Log("Sequence �ڷ�ƾ ����");

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
                /*
                effect.TargetScript.GetType
                yield return effect.MethodName;
                */
            }
        }

        IsPlaying = false;

        // �÷��̾� ���� ���� ����
        SceneContext.Current.Player.IsGodMode = false;
    }

    /// <summary>
    /// �������� ����Ѵ�
    /// </summary>
    public void Play()
    {
        if (!_playOnce || !_played)
        {
            SceneEffectManager.Instance.PushCutscene(new Cutscene(this, PlaySequenceCoroutine(_sequence)));
            _played = true;

            // �÷��̾ ���� ���·� �����
            SceneContext.Current.Player.IsGodMode = true;
            // Debug.Log($"{SceneContext.Current.Player}�� GodMode�� �����˴ϴ�. => IsGodMode : {SceneContext.Current.Player.IsGodMode}");
        }
        else
        {
            InputManager.Instance.ChangeToDefaultSetter();
        }
    }

    /// <summary>
    /// Ʈ���ŷ� ���� ������ ���
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
