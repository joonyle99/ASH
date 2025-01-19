using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Stay Still InputSetter", menuName = "InputSetters/Dialogue")]
public class DialogueStayStillInputSetter : InputSetterScriptableObject
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
