using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D.TopDownShooter;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// �÷��̾ ��ȣ�ۿ� �ϴ� ��쿡 �����ϴ� ����
/// Enum ������ ������ �ִϸ��̼� �� ������ �����Ѵ�
/// </summary>
public class InteractionState : PlayerState
{
    // ��ȣ�ۿ� ���� Ÿ�� ������Ʈ
    [SerializeField] private InteractableObject _targetObject = null;

    // ������ Interaction Type
    [SerializeField] private InteractionType _curInteractionType = InteractionType.None;

    #region PUSH

    #endregion

    #region ROLL

    [SerializeField] private float _rollPower = 5f;
    [SerializeField] private float _rollMaxSpeed = 0.8f;

    #endregion

    protected override void OnEnter()
    {
        SetTargetObject(GetComponent<InteractionController>().InteractionTarget);
        _curInteractionType = _targetObject.InteractionTypeWithPlayer;
        // Animation Parmeter ����
        switch (_curInteractionType)
        {
            case InteractionType.None:
                Debug.LogWarning("Interaction not set");
                break;
            case InteractionType.Push:
                Player.Animator.SetBool("IsPush", true);
                break;
            case InteractionType.Roll:
                Player.Animator.SetBool("IsPush", true);
                Rigidbody2D targetRigid = _targetObject.GetComponent<Rigidbody2D>();
                if (targetRigid == null)
                    return;
                _rollPower = targetRigid.mass * 1.1f * Player.Rigidbody.gravityScale;
                break;
        }
    }

    protected override void OnUpdate()
    {
        // Debug.Log("OnUpdate Interaction State");

        switch (_curInteractionType)
        {
            case InteractionType.None:
            case InteractionType.Push:
                break;
            case InteractionType.Roll:

                if (Mathf.Abs(Player.Rigidbody.velocity.x) > _rollMaxSpeed)
                    Player.Rigidbody.velocity = new Vector2(_rollMaxSpeed * Mathf.Sign(Player.Rigidbody.velocity.x),
                        Player.Rigidbody.velocity.y);

                break;
        }
    }

    protected override void OnFixedUpdate()
    {
        // Debug.Log("OnFixedUpdate Interaction State");

        switch (_curInteractionType)
        {
            case InteractionType.None:
            case InteractionType.Push:
                break;
            case InteractionType.Roll:

                // ������Ʈ�� �ƴ� �÷��̾� �̵��� ���� �����Ų��
                // ��ȣ�ۿ� ���� ������Ʈ�� ���Ա��� ���� �̵��� ���
                // �÷��̾��� GravitySclae�� 1�� �ƴ� 5�� �����Ǿ��ִ�. �� ���� ����ؼ� ���� ����Ѵ�.

                Player.Rigidbody.AddForce(Vector2.right * _rollPower * Player.RawInputs.Movement.x);

                break;
        }
    }

    protected override void OnExit()
    {
        // Animation Parmeter �ʱ�ȭ
        switch (_curInteractionType)
        {
            case InteractionType.None:
                break;
            case InteractionType.Push:
            case InteractionType.Roll:
                Player.Animator.SetBool("IsPush", false);
                break;
        }

        Finalize();
    }

    public void SetTargetObject(InteractableObject targetObject)
    {
        if (targetObject == null)
        {
            Debug.Log("Target Object�� �����ϴ�");
            return;
        }

        _targetObject = targetObject;

        return;
    }

    public void Finalize()
    {
        _targetObject = null;
        _curInteractionType = InteractionType.None;
        _rollPower = 0f;

        return;
    }
}
