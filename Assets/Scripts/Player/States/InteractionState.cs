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

    [SerializeField] private float _rollingPower = 5f;
    [SerializeField] private float _rollingMaxSpeed = 0.8f;

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
                Player.Animator.SetBool("IsPush", true);
                break;
            case InteractionType.Roll:
                Player.Animator.SetBool("IsPush", true);

                // rollingPower ����
                Rigidbody2D targetRigid = _targetObject.GetComponent<Rigidbody2D>();

                if (targetRigid == null)
                    return;

                // �÷��̾��� GravitySclae�� 1�� �ƴ� 5�� �����Ǿ��ִ�. �� ���� ����ؼ� ���� ����Ѵ�.
                //_rollingPower = targetRigid.mass * 1.1f * Player.Rigidbody.gravityScale;

                break;
        }
    }

    /// <summary>
    /// Game Logic
    /// </summary>
    protected override void OnUpdate()
    {
        // Debug.Log("OnUpdate Interaction State");
        return;
        switch (_curInteractionType)
        {
            case InteractionType.Push:
                break;
            case InteractionType.Roll:

                if (Mathf.Abs(Player.Rigidbody.velocity.x) > _rollingMaxSpeed)
                    Player.Rigidbody.velocity = new Vector2(_rollingMaxSpeed * Mathf.Sign(Player.Rigidbody.velocity.x),
                        Player.Rigidbody.velocity.y);

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
            Debug.Log("Target Object�� �����ϴ�");
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
