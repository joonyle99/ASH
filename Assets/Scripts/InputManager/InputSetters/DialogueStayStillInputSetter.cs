using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Stay Still InputSetter", menuName = "InputSetters/Dialogue")]
public class DialogueStayStillInputSetter : InputSetterScriptableObject
{
    [SerializeField] KeyCode _dialogueKey;
    public override InputState GetState()
    {
        InputState state = new InputState();
        state.Movement = Vector2.zero;
        if (DialogueController.Instance.IsDialogueActive)
            state.InteractionKey.Update(_dialogueKey);
        return state;
    }

}
