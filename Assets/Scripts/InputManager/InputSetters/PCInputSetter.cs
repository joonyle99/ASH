using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Default PC InputSetter", menuName = "InputSetters/PCDefault")]
public class PCInputSetter : InputSetterScriptableObject, IInputSetter
{
    [SerializeField] KeyCode _jumpKeyCode = KeyCode.Space;
    [SerializeField] KeyCode _dashKeyCode = KeyCode.F;
    [SerializeField] KeyCode _basicAttackKeyCode = KeyCode.A;
    [SerializeField] KeyCode _shootingAttackKeyCode = KeyCode.S;
    [SerializeField] KeyCode _interactionKeyCode = KeyCode.E;
    [SerializeField] KeyCode _lightingKeyCode = KeyCode.L;

    public override InputState GetState()
    {
        // "매 프레임" 새로운 Input Data를 생성
        InputState state = new InputState();

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            state.Movement.x -= 1;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            state.Movement.x += 1;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            state.Movement.y += 1;
            state.Vertical += 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            state.Movement.y -= 1;
            state.Vertical -= 1;
        }

        state.InteractionKey.Update(_interactionKeyCode);
        state.JumpKey.Update(_jumpKeyCode);
        state.DashKey.Update(_dashKeyCode);
        state.BasicAttackKey.Update(_basicAttackKeyCode);
        state.ShootingAttackKey.Update(_shootingAttackKeyCode);
        state.LightKey.Update(_lightingKeyCode);

        return state;
    }
}
