using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PushableTree : InteractableObject
{
    public FallingTreeByPush FallingTreeByPush;

    protected override void OnInteract()
    {
        // Debug.Log("�������� ��ȣ�ۿ� ����");

    }

    public override void UpdateInteracting()
    {
        InputState inputState = InputManager.Instance.GetState();

        // ��ȣ�ۿ� ���� Ÿ�̹�
        if (InputManager.Instance.InteractionKey.KeyUp || inputState.Horizontal < 0.1f || FallingTreeByPush.IsFalling)
        {
            // ��ȣ�ۿ� ���� Ÿ�̹�
            if (FallingTreeByPush.IsFalling)
            {
                // ���̻� ��ȣ�ۿ� ���ϰ� ���´�
                IsInteractable = false;
            }

            // ��ȣ�ۿ� ����
            FallingTreeByPush.FinishPush();
            FinishInteraction();

            return;
        }

        // Push Dir ����
        float pushDir = Mathf.Sign(FallingTreeByPush.transform.position.x - SceneContext.Current.Player.transform.position.x);

        // ���������� �о���ϴ� ���
        if (pushDir > 0)
        {
            // ������ ����Ű�� ������
            if (inputState.Horizontal > 0f)
            {
                // Top Tree �κ��� �δ�.
                FallingTreeByPush.ExcutePush(pushDir);
            }
        }
        // �������� �о���ϴ� ���
        else if (pushDir < 0)
        {
            // ���� ����Ű�� ������
            if (inputState.Horizontal < 0f)
            {
                // Top Tree �κ��� �δ�.
                FallingTreeByPush.ExcutePush(pushDir);
            }
        }
    }
}