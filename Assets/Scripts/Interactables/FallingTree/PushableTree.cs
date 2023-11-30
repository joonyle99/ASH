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
        if (InputManager.Instance.InteractionKey.KeyUp || FallingTreeByPush.IsFalling)
        {
            // ��ȣ�ۿ� ���� Ÿ�̹� (������ �̹� ������ ���)
            if (FallingTreeByPush.IsFalling)
            {
                // ���̻� ��ȣ�ۿ� ���ϰ� ���´�
                IsInteractable = false;
            }

            // ��ȣ�ۿ� ����
            FallingTreeByPush.StopPush();
            FinishInteraction();

            return;
        }

        if (Mathf.Abs(inputState.Horizontal) < 0.1f)
        {
            // -------------------------------------------- //
            //      ������ ���� �ʰ� ������ �ִ� �ڵ�         //
            // -------------------------------------------- //

            // Debug.Log("������ ������ ��� �ִ´�");

            FallingTreeByPush.StopPush();
        }
        else
        {

            // -------------------------------------------- //
            //      ������ �÷��̾ ������ �̴� �ڵ�         //
            // -------------------------------------------- //

            // Debug.Log("������ �δ�");

            // Push Dir ����
            float pushDir = Mathf.Sign(FallingTreeByPush.transform.position.x - SceneContext.Current.Player.transform.position.x);

            if (pushDir > 0f)
            {
                if (inputState.Horizontal > 0f)
                {
                    // Top Tree �κ��� �δ�.
                    FallingTreeByPush.StartPush(pushDir);
                }
            }
            else if (pushDir < 0f)
            {
                if (inputState.Horizontal < 0f)
                {
                    FallingTreeByPush.StartPush(pushDir);
                }
            }
        }
    }
}