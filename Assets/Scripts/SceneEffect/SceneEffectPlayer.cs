using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class SceneEffectPlayer : MonoBehaviour, ITriggerListener
{
    [SerializeField] bool _playEnterEffectOnce = true;
    [SerializeField] CameraPriority _enterCameraEffectPriority = CameraPriority.Cutscene;
    [SerializeField] List<SceneEffect> _enterSequence;

    bool _enterEffectPlayed = false;
    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter) 
    {
        if (activator.Type != ActivatorType.Player)
            return;
        if (_playEnterEffectOnce && _enterEffectPlayed)
            return;

        _enterEffectPlayed = true;
        StartCoroutine(PlaySequenceCoroutine(_enterSequence));
    }
    public void OnExitReported(TriggerActivator activator, TriggerReporter reporter) { }
    public void OnStayReported(TriggerActivator activator, TriggerReporter reporter) { }

    IEnumerator PlaySequenceCoroutine(List<SceneEffect> sequence)
    {
        for(int i=0; i<sequence.Count; i++)
        {
            var effect = sequence[i];
            
            if (effect.IsCameraEffect)
            {
                CameraControlToken token = new CameraControlToken(_enterCameraEffectPriority);
                yield return new WaitUntil(() => token.IsAvailable);
                if (effect.Type == SceneEffect.EffectType.CameraShake)
                {
                    token.Camera.StartShake(effect.ShakeData);
                }
                else if (effect.Type == SceneEffect.EffectType.ConstantCameraShake)
                {
                    token.Camera.StartConstantShake(effect.ConstantShakeData);
                }
                else if (effect.Type == SceneEffect.EffectType.StopConstantCameraShake)
                {
                    token.Camera.StopConstantShake(effect.Time);
                }
                if (i == sequence.Count-1 || !sequence[i + 1].IsCameraEffect)
                    token.Release();
            }
            else if (effect.Type == SceneEffect.EffectType.Dialogue)
            {
                DialogueController.Instance.StartDialogue(effect.DialogueData);
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
        }
    }

}
