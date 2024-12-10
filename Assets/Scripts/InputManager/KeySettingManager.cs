using HappyTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySettingManager
{
    private static HashSet<KeyCode> _unusableKey = new HashSet<KeyCode>() { KeyCode.LeftApple };

    public static void SetKeyboardSetting(InputSetterDataType inputSetterDataType, string keyName, KeyCode newKeyCode)
    {
        switch (inputSetterDataType)
        {
            case InputSetterDataType.None:
                {
                    break;
                }
            case InputSetterDataType.DefaultInputSetter:
                {
                    PCInputSetter pcInputSetter = InputManager.Instance?.DefaultInputSetter as PCInputSetter;
                    pcInputSetter.SetKeyCode(keyName, newKeyCode);
                    break;
                }
            case InputSetterDataType.MoveRightInputSetter: 
                break;
            case InputSetterDataType.MoveLeftInputSetter: 
                break;
            case InputSetterDataType.StayStillInputSetter:
                StayStillInputSetter SSInputSetter = InputManager.Instance?.StayStillInputSetter as StayStillInputSetter;
                SSInputSetter.SetKeyCode(keyName, newKeyCode);
                break;
        }
    }

    /// <summary>
    /// 검사할 InputSetter가 추가되면 해당 함수의 케이스를 추가해 주어야 함
    /// </summary>
    /// <param name="name"></param>
    /// <param name="keyCode"></param>
    /// <returns></returns>
    public static ChangeKeySettingErrorReason CheckKeyCode(string name, KeyCode keyCode)
    {
        if (_unusableKey.Contains(keyCode)) return ChangeKeySettingErrorReason.UnusableKey;


        PCInputSetter pCInputSetter = InputManager.Instance.DefaultInputSetter as PCInputSetter;
        foreach (var pair in pCInputSetter.KeyCodes)
        {
            if (pair.Value.KeyCode == keyCode)
            {
                return ChangeKeySettingErrorReason.AlreadyUsing;
            }
        }

        return ChangeKeySettingErrorReason.None;
    }
}
