using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class CutscenePlayer : MonoBehaviour, ITriggerListener
{
    List<CutsceneDataComponent> _cutsceneDatas;
    
    void Awake()
    {
        _cutsceneDatas = new List<CutsceneDataComponent>(GetComponents<CutsceneDataComponent>());
    }
    public void PlayCutscene(string cutsceneName)
    {
        var cutscene = _cutsceneDatas.Find(x=>x.CutsceneName == cutsceneName);
        SceneEffectManager.Current.PushCutscene(new Cutscene(this, PlaySequenceCoroutine(cutscene.Sequence)));
    }
    IEnumerator PlaySequenceCoroutine(List<SceneEffect> sequence)
    {
        for(int i=0; i<sequence.Count; i++)
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
            else if (effect.Type == SceneEffect.EffectType.FunctionCall)
            {
                effect.Function?.Invoke();
            }
        }
    }

}
