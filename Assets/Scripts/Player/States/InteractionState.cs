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
    [SerializeField] private InteractionType.Type _curInteractionType = InteractionType.Type.NULL;

    #region PUSH

    #endregion

    #region ROLL

    [SerializeField] private float _rollPower = 5f;
    [SerializeField] private float _rollMaxSpeed = 0.8f;

    #endregion

    protected override void OnEnter()
    {
        Debug.Log("Enter Interaction State");

        // Target Object ��������
        SetTargetObject(this.GetComponent<InteractionController>().GetTargetObject());

        // Animation Parmeter ����
        switch (_curInteractionType)
        {
            case InteractionType.Type.NULL:
                Debug.LogError("��ȣ�ۿ� Ÿ���� �Էµ��� �ʾҽ��ϴ�");
                break;
            case InteractionType.Type.PUSH:
            case InteractionType.Type.ROLL:
                Rigidbody2D targetRigid = _targetObject.GetComponent<Rigidbody2D>();
                if (targetRigid == null)
                    return;
                _rollPower = targetRigid.mass * 1.1f * Player.Rigidbody.gravityScale;
                Player.Animator.SetBool("IsPush", true);
                break;
        }
    }

    protected override void OnUpdate()
    {
        // Debug.Log("OnUpdate Interaction State");

        switch (_curInteractionType)
        {
            case InteractionType.Type.NULL:
            case InteractionType.Type.PUSH:
                break;
            case InteractionType.Type.ROLL:

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
            case InteractionType.Type.NULL:
            case InteractionType.Type.PUSH:
                break;
            case InteractionType.Type.ROLL:

                // ������Ʈ�� �ƴ� �÷��̾� �̵��� ���� �����Ų��
                // ��ȣ�ۿ� ���� ������Ʈ�� ���Ա��� ���� �̵��� ���
                // �÷��̾��� GravitySclae�� 1�� �ƴ� 5�� �����Ǿ��ִ�. �� ���� ����ؼ� ���� ����Ѵ�.

                Player.Rigidbody.AddForce(Vector2.right * _rollPower * Player.RawInputs.Movement.x);

                break;
        }
    }

    protected override void OnExit()
    {
        Debug.Log("OnExit Interaction State");

        // Animation Parmeter �ʱ�ȭ
        switch (_curInteractionType)
        {
            case InteractionType.Type.NULL:
                break;
            case InteractionType.Type.PUSH:
            case InteractionType.Type.ROLL:
                Player.Animator.SetBool("IsPush", false);
                break;
        }

        Finalize();
    }

    public void SetInteractionType(InteractionType.Type type)
    {
        _curInteractionType = type;
        return;
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
        _curInteractionType = InteractionType.Type.NULL;
        _rollPower = 0f;

        return;
    }
}
