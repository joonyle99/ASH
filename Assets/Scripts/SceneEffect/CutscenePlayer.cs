using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutscenePlayer : MonoBehaviour, ITriggerListener
{
    [SerializeField] bool _playOnce = true;
    [SerializeField] List<SceneEffect> _sequence;

    bool _played = false;

    public bool IsPlaying { get; private set; } = false;

    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter)
    {
        if (activator.Type == ActivatorType.Player)
            Play();
    }
    public void Play()
    {
        if (!_played && _playOnce)
        {
            SceneEffectManager.Current.PushCutscene(new Cutscene(this, PlaySequenceCoroutine(_sequence)));
            _played = true;
        }
    }
    IEnumerator PlaySequenceCoroutine(List<SceneEffect> sequence)
    {
        IsPlaying = true;
        for (int i=0; i<sequence.Count; i++)
        {
            var effect = sequence[i];

            if (effect.Type == SceneEffect.EffectType.CameraShake)
            {
                SceneEffectManager.Current.Camera.StartShake(effect.ShakeData);
            }
            else if (effect.Type == SceneEffect.EffectType.ConstantCameraShake)
            {
                SceneEffectManager.Current.Camera.StartConstantShake(effect.ConstantShakeData);
            }
            else if (effect.Type == SceneEffect.EffectType.StopConstantCameraShake)
            {
                SceneEffectManager.Current.Camera.StopConstantShake(effect.Time);
            }
            else if (effect.Type == SceneEffect.EffectType.Dialogue)
            {
                DialogueController.Instance.StartDialogue(effect.DialogueData, true);
                yield return new WaitWhile(() => DialogueController.Instance.IsDialogueActive);
            }
            else if (effect.Type == SceneEffect.EffectType.LifePurchase)
            {
                GameUIManager.OpenLifePurchasePanel();
                yield return new WaitWhile(() => GameUIManager.IsLifePurchasePanelOpen);
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
        }
        IsPlaying = false;
    }

}
