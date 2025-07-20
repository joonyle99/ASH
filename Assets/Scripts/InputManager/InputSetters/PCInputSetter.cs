using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Default PC InputSetter", menuName = "InputSetters/PCDefault")]
public class PCInputSetter : InputSetterScriptableObject, IInputSetter
{
    private void Awake()
    {
        _keyCodes = new List<DataDictionary<string, CustomKeyCode>>
        {
            new DataDictionary<string, CustomKeyCode>() { Key = "위쪽이동", Value = new CustomKeyCode("위쪽이동","ui_moveToupper", KeyCode.UpArrow) },
            new DataDictionary<string, CustomKeyCode>() { Key = "아래쪽이동", Value = new CustomKeyCode("아래쪽이동","ui_moveToLower", KeyCode.DownArrow) },
            new DataDictionary<string, CustomKeyCode>() { Key = "왼쪽이동", Value = new CustomKeyCode("왼쪽이동","ui_moveToLeft", KeyCode.LeftArrow) },
            new DataDictionary<string, CustomKeyCode>() { Key = "오른쪽이동", Value = new CustomKeyCode("오른쪽이동","ui_moveToRight", KeyCode.RightArrow) },
            new DataDictionary<string, CustomKeyCode>() { Key = "Jump", Value = new CustomKeyCode("Jump", "ui_jump", KeyCode.Space) },
            new DataDictionary<string, CustomKeyCode>() { Key = "Dash", Value = new CustomKeyCode("Dash", "ui_dash", KeyCode.LeftShift) },
            new DataDictionary<string, CustomKeyCode>() { Key = "상호작용", Value = new CustomKeyCode("상호작용","ui_interaction", KeyCode.F) },
            new DataDictionary<string, CustomKeyCode>() { Key = "LightSkill", Value = new CustomKeyCode("LightSkill", "ui_lightSkill", KeyCode.Q) },
            new DataDictionary<string, CustomKeyCode>() { Key = "기본공격", Value = new CustomKeyCode("기본공격", "ui_attack", KeyCode.A) },
        };
    }

    public override InputState GetState()
    {
        // "매 프레임" 새로운 Input Data를 생성
        InputState state = base.GetState();

        if (Input.GetKey(GetKeyCode("오른쪽이동").KeyCode))
        {
            state.Movement.x += 1;
        }
        if (Input.GetKey(GetKeyCode("왼쪽이동").KeyCode))
        {
            state.Movement.x -= 1;
        }
        if (Input.GetKey(GetKeyCode("위쪽이동").KeyCode))
        {
            state.Movement.y += 1;
            state.Vertical += 1;
        }
        if (Input.GetKey(GetKeyCode("아래쪽이동").KeyCode))
        {
            state.Movement.y -= 1;
            state.Vertical -= 1;
        }

        state.InteractionKey.Update(GetKeyCode("상호작용").KeyCode);
        state.JumpKey.Update(GetKeyCode("Jump").KeyCode);
        state.DashKey.Update(GetKeyCode("Dash").KeyCode);
        state.AttackKey.Update(GetKeyCode("기본공격").KeyCode);
        state.LightKey.Update(GetKeyCode("LightSkill").KeyCode);

        return state;
    }
}
