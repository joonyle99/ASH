using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ƾ� �÷��̾�� �پ��� ������ �������� ���� ����Ѵ�
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
    /// �������� �´� ������ ����Ѵ�
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    private IEnumerator PlaySequenceCoroutine(List<SceneEffect> sequence)
    {
        Debug.Log("Sequence �ڷ�ƾ ����");

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

        // �÷��̾� ���� ���� ����
        SceneContext.Current.Player.IsGodMode = false;
        // Debug.Log($"{SceneContext.Current.Player}�� GodMode�� �����˴ϴ�. => IsGodMode : {SceneContext.Current.Player.IsGodMode}");

        Debug.Log("Sequence �ڷ�ƾ ����");
    }

    /// <summary>
    /// �������� ����Ѵ�
    /// </summary>
    public void Play()
    {
        if (!_played && _playOnce)
        {
            SceneEffectManager.Instance.PushCutscene(new Cutscene(this, PlaySequenceCoroutine(_sequence)));
            _played = true;

            // �÷��̾ ���� ���·� �����
            SceneContext.Current.Player.IsGodMode = true;
            // Debug.Log($"{SceneContext.Current.Player}�� GodMode�� �����˴ϴ�. => IsGodMode : {SceneContext.Current.Player.IsGodMode}");
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
}
