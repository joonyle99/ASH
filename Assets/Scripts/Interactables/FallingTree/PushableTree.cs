using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PushableTree : InteractableObject
{
    public FallingTreeByPush FallingTreeByPush;

    protected override void OnInteract()
    {

    }

    public override void UpdateInteracting()
    {
        InputState inputState = InputManager.Instance.GetState();

        // ��ȣ�ۿ� ���� Ÿ�̹�
        if (InputManager.Instance.InteractionKey.KeyUp || FallingTreeByPush.IsFallingEnd)
        {
            // ������ �̹� ������ ���
            if (FallingTreeByPush.IsFallingEnd)
            {
                // ���̻� ��ȣ�ۿ� ���ϰ� ���´�
                IsInteractable = false;
            }

            // ��ȣ�ۿ� ����
            FallingTreeByPush.StopPush();
            FinishInteraction();

            return;
        }

        // �÷��̾ �����̸� + �÷��̾ �������̸� -
        float pushDir = Mathf.Sign(FallingTreeByPush.transform.position.x - SceneContext.Current.Player.transform.position.x);

        // ���� �������� ���
        bool isSyncDir = Mathf.Abs(pushDir - inputState.Horizontal) < 0.01f;

        // ���� ������ ��쿡�� �δ�.
        if (isSyncDir)
            FallingTreeByPush.StartPush(pushDir);
        else
            FallingTreeByPush.StopPush();

    }
}