using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D.TopDownShooter;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

/// <summary>
/// �÷��̾ ��ȣ�ۿ� �ϴ� ��쿡 �����ϴ� ����
/// Enum ������ ������ �ִϸ��̼� �� ������ �����Ѵ�
/// </summary>
public class InteractionState : PlayerState
{
    // ��ȣ�ۿ� ���� Ÿ�� ������Ʈ
    [SerializeField] private InteractableObject _targetObject = null;

    // ������ Interaction MonsterType
    [SerializeField] private InteractionType _curInteractionType = InteractionType.None;

    #region PUSH

    #endregion

    #region ROLL

    [SerializeField] private float _rollingPower;

    #endregion

    protected override void OnEnter()
    {
        // Debug.Log("OnEnter InteractionState");

        Player.Animator.SetTrigger("Interact");

        // Ÿ�� ������Ʈ�� ��ȣ�ۿ� Ÿ�� ����
        SetTargetObject(this.GetComponent<InteractionController>().InteractionTarget);
        SetInteractionType(_targetObject.InteractionTypeWithPlayer);

        // �ִϸ��̼� �Ķ���� ����
        switch (_curInteractionType)
        {
            case InteractionType.Push:
            case InteractionType.Roll:
                Player.Animator.SetBool("IsPush", true);
                break;
        }
    }

    protected override void OnUpdate()
    {
        // Debug.Log("OnUpdate Interaction State");

        switch (_curInteractionType)
        {
            case InteractionType.Push:
                break;
            case InteractionType.Roll:
                break;
        }
    }

    /// <summary>
    /// Physics
    /// </summary>
    protected override void OnFixedUpdate()
    {
        // Debug.Log("OnFixedUpdate Interaction State");

        switch (_curInteractionType)
        {
            case InteractionType.Push:
                break;
            case InteractionType.Roll:

                // ������Ʈ�� �ƴ� �÷��̾� �̵��� ���� �����Ų��
                // ��ȣ�ۿ� ���� ������Ʈ�� ���Ա��� ���� �̵��� ���
                Player.Rigidbody.AddForce(Player.RawInputs.Movement * _rollingPower);
                break;
        }
    }

    protected override void OnExit()
    {
        // Debug.Log("OnExit InteractionState");

        // �ִϸ��̼� �Ķ���� �ʱ�ȭ
        switch (_curInteractionType)
        {
            case InteractionType.Push:
            case InteractionType.Roll:
                Player.Animator.SetBool("IsPush", false);
                break;
        }

        FinalizeInteraction();
    }

    public void SetTargetObject(InteractableObject targetObject)
    {
        if (targetObject == null)
        {
            Debug.LogWarning("Target Object is null");
            return;
        }

        _targetObject = targetObject;
    }

    public void SetInteractionType(InteractionType interactionType)
    {
        _curInteractionType = interactionType;

        if (interactionType == InteractionType.None)
            Debug.LogWarning("Interaction not set");
    }

    public void FinalizeInteraction()
    {
        _targetObject = null;
        _curInteractionType = InteractionType.None;
    }
}
