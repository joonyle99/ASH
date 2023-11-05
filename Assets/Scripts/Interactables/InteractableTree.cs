using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableTree : InteractableObject
{
    public TopOfTree topOfTree;

    protected override void OnInteract()
    {
        Debug.Log("�������� ��ȣ�ۿ� ����");

        // �÷��̾ ��ȣ�ۿ� : Push State��
        // SceneContext.Current.Player.GetComponent<PlayerBehaviour>().ChangeState<PushState>();
    }
    public override void UpdateInteracting()
    {
        // ���� ������ ����Ѵ�
        if (InputManager.Instance.InteractionKey.KeyUp)
        {
            topOfTree.FinishPush();
            FinishInteraction();

            return;
        }

        // Top Tree �κ��� �δ�.
        float dir = Mathf.Sign(topOfTree.transform.position.x - SceneContext.Current.Player.transform.position.x);
        topOfTree.ExcutePush(dir);
    }

}