using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾ ��ȣ�ۿ� �ϴ� ��쿡 �����ϴ� ����
/// Enum ������ ������ �ִϸ��̼� �� ������ �����Ѵ�
/// </summary>
public class InteractionState : PlayerState
{
    // ������ Interaction Type
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
