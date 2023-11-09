using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 상호작용 하는 경우에 진입하는 상태
/// Enum 정보를 가지고 애니메이션 및 동작을 적용한다
/// </summary>
public class InteractionState : PlayerState
{
    // 현재의 Interaction Type
    public InteractionType.Type curInteractionType = InteractionType.Type.NULL;

    protected override void OnEnter()
    {
        Debug.Log("Enter Interaction State");
        
        Player.Animator.SetBool("IsPush", true);
    }

    protected override void OnUpdate()
    {
        Debug.Log("OnUpdate Interaction State");
    }

    protected override void OnFixedUpdate()
    {

    }

    protected override void OnExit()
    {
        Debug.Log("OnExit Interaction State");

        InitInteractionType();

        Player.Animator.SetBool("IsPush", false);
    }

    public InteractionType.Type ChangeInteractionType(InteractionType.Type type)
    {
        curInteractionType = type;

        return curInteractionType;
    }

    public void InitInteractionType()
    {
        curInteractionType = InteractionType.Type.NULL;
    }
}
