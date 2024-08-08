using UnityEngine;

[CreateAssetMenu(fileName = "New Stay Still InputSetter", menuName = "InputSetters/Stay Still")]
public class StayStillInputSetter : InputSetterScriptableObject
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
