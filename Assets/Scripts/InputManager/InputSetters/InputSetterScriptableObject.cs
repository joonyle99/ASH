using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum InputSetterDataType
{
    None,
    DefaultInputSetter,
    MoveRightInputSetter,
    MoveLeftInputSetter,
    StayStillInputSetter,
    OnlyLightInputSetter,
}

[Serializable]
public class CustomKeyCode
{
    public string Name;
    public KeyCode KeyCode;

    public CustomKeyCode(string name, KeyCode keyCode)
    {
        Name = name;
        KeyCode = keyCode;
    }
}

public abstract class InputSetterScriptableObject : ScriptableObject, IInputSetter
{
    [SerializeField] protected List<DataDictionary<string, CustomKeyCode>> _keyCodes;
    public List<DataDictionary<string, CustomKeyCode>> KeyCodes => _keyCodes;


    [SerializeField] protected string _inputSetterDescription;
    public string InputSetterDescription => _inputSetterDescription;


    [SerializeField] protected InputSetterDataType _inputSetterDataType = InputSetterDataType.None;
    public InputSetterDataType InputSetterDataType => _inputSetterDataType;


    [SerializeField] protected KeyCode escapeKeyCode = KeyCode.Escape;

    public virtual InputState GetState()
    {
        InputState state = new InputState();
        state.EscapeKey.Update(escapeKeyCode);
        return state;
    }

    public virtual void Update() { }

    public virtual void SetKeyCode(string keyName, KeyCode newKeyCode)
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

    public CustomKeyCode GetKeyCode(string name)
    {
        for (int i = 0; i < _keyCodes.Count; i++)
        {
            DataDictionary<string, CustomKeyCode> dict = _keyCodes[i];

            if (dict.Key.Equals(name))
            {
                return dict.Value;
            }
        }

        return null;
    }
}
