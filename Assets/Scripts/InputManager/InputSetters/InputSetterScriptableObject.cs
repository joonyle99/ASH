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
    public string Key;

    //해당 키의 설명
    public string NameKey;
    public KeyCode KeyCode;

    public CustomKeyCode(string key, string nameKey, KeyCode keyCode)
    {
        Key = key;
        NameKey = nameKey;
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

    public virtual void SetKeyCode(string key, KeyCode newKeyCode)
    {
        CustomKeyCode targetKeyCode = GetKeyCode(key);

        if (targetKeyCode == null)
        {
            Debug.Log(key + " not exist keyname");
            return;
        }
        Debug.Log(key + " change key to " + newKeyCode);

        targetKeyCode.KeyCode = newKeyCode;
    }

    public CustomKeyCode GetKeyCode(string key)
    {
        for (int i = 0; i < _keyCodes.Count; i++)
        {
            DataDictionary<string, CustomKeyCode> dict = _keyCodes[i];

            if (dict.Key.Equals(key))
            {
                return dict.Value;
            }
        }

        return null;
    }

    public CustomKeyCode GetCustomKeyCodeByKeyCode(KeyCode keyCode)
    {
        for(int i = 0; i < _keyCodes.Count; i++)
        {
            if(_keyCodes[i].Value.KeyCode == keyCode)
            {
                return _keyCodes[i].Value;
            }
        }

        return null;
    }
}
