using UnityEngine;

[CreateAssetMenu(fileName = "New Stay Still InputSetter", menuName = "InputSetters/Stay Still")]
public class StayStillInputSetter : InputSetterScriptableObject
{
    [SerializeField] KeyCode _dialogueKey;
    public override InputState GetState()
    {
        InputState state = base.GetState();

        state.Movement = Vector2.zero;
        if (DialogueController.Instance.IsDialogueActive)
            state.InteractionKey.Update(_dialogueKey);

        return state;
    }

    public override void SetKeyCode(string keyName, KeyCode newKeyCode)
    {
        CustomKeyCode targetKeyCode = GetKeyCode(keyName);

        if (targetKeyCode == null)
        {
            Debug.Log(keyName + " not exist keyname");
            return;
        }
        Debug.Log(keyName + " change key to " + newKeyCode);

        targetKeyCode.KeyCode = newKeyCode;
    }
}
