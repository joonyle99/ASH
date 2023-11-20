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
        // ��ȣ�ۿ� ���� Ÿ�̹�
        if (InputManager.Instance.InteractionKey.KeyUp || FallingTreeByPush.IsFalling)
        {
            // ��ȣ�ۿ� ����
            FallingTreeByPush.FinishPush();
            FinishInteraction();

            // ���̻� ��ȣ�ۿ� ���ϰ� ���´�
            IsInteractable = false;

            return;
        }

        // Top Tree �κ��� �δ�.
        float dir = Mathf.Sign(FallingTreeByPush.transform.position.x - SceneContext.Current.Player.transform.position.x);
        FallingTreeByPush.ExcutePush(dir);
    }
}