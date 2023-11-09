using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableTree : InteractableObject
{
    public FallingTopTree FallingTopTree;

    // Own Interaction Type ����
    public InteractionType.Type ownInteractionType = InteractionType.Type.PUSH;

    protected override void OnInteract()
    {
        // Debug.Log("�������� ��ȣ�ۿ� ����");

        // Player���� Own Interaction Type�� �Ѱ��ش�
        SceneContext.Current.Player.GetComponent<InteractionState>().ChangeInteractionType(ownInteractionType);
    }

    public override void UpdateInteracting()
    {
        // ��ȣ�ۿ� ���� Ÿ�̹�
        if (InputManager.Instance.InteractionKey.KeyUp || FallingTopTree.IsFalling)
        {
            // ��ȣ�ۿ� ����
            FallingTopTree.FinishPush();
            FinishInteraction();

            return;
        }

        // Top Tree �κ��� �δ�.
        float dir = Mathf.Sign(FallingTopTree.transform.position.x - SceneContext.Current.Player.transform.position.x);
        FallingTopTree.ExcutePush(dir);
    }

}