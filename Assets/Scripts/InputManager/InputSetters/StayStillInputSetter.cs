using UnityEngine;

[CreateAssetMenu(fileName = "New Stay Still InputSetter", menuName = "InputSetters/Stay Still")]
public class StayStillInputSetter : InputSetterScriptableObject
{
    public override InputState GetState()
    {
        InputState state = base.GetState();

        state.Movement = Vector2.zero;
        if (DialogueController.Instance.IsDialogueActive)
            state.InteractionKey.Update(GetKeyCode("Dialogue").KeyCode);

        return state;
    }
}
