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

        float pushDir = Mathf.Sign(FallingTreeByPush.transform.position.x - SceneContext.Current.Player.transform.position.x);
        bool isSync = Mathf.Abs(pushDir - inputState.Horizontal) < 0.01f;

        if (!isSync)
            FallingTreeByPush.StopPush();
        else
            FallingTreeByPush.StartPush(pushDir);
    }
}