using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New OnlyLight InputSetter", menuName = "InputSetters/OnlyLight")]
public class OnlyLightInputSetter : InputSetterScriptableObject, IInputSetter
{
    private void Awake()
    {
        _keyCodes = new List<DataDictionary<string, CustomKeyCode>>
        {
            new DataDictionary<string, CustomKeyCode>() { Key = "위쪽이동", Value = new CustomKeyCode("위쪽이동", KeyCode.UpArrow) },
            new DataDictionary<string, CustomKeyCode>() { Key = "아래쪽이동", Value = new CustomKeyCode("아래쪽이동", KeyCode.DownArrow) },
            new DataDictionary<string, CustomKeyCode>() { Key = "LightSkill", Value = new CustomKeyCode("LightSkill", KeyCode.Q) },
        };
    }

    public override InputState GetState()
    {
        // "매 프레임" 새로운 Input Data를 생성
        InputState state = base.GetState();

        if (Input.GetKey(GetKeyCode("위쪽이동").KeyCode))
        {
            state.Vertical += 1;
        }
        if (Input.GetKey(GetKeyCode("아래쪽이동").KeyCode))
        {
            state.Vertical -= 1;
        }

        state.LightKey.Update(GetKeyCode("LightSkill").KeyCode);

        return state;
    }
}
