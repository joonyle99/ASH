using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableTree : InteractableObject
{
    public GameObject topOfTree;

    protected override void OnInteract()
    {
        Debug.Log("�̰��� �������� ��ȣ�ۿ� �Դϴ�. -> �̸��� : " + topOfTree.name);

        // ���⼭ ������ �о���� �ϴµ�..

        // �÷��̾� ������ �� �� �ֳ�?

        // topOfTree.GetComponent<FallingDownTree>().FallingDown();
    }
    public override void UpdateInteracting()
    {
        throw new System.NotImplementedException();
        /*
         * InteractableObject.Update�� �ִ� �ڵ�
         * 
        // �������� �������� ��ȣ�ۿ�
        if (Input.GetKey(_interactionKey))
        {
            GameObject topTree = (_interactionTarget as InteractableTree).topOfTree;

            float dir = Mathf.Sign(topTree.transform.position.x - this.transform.position.x);

            topTree.GetComponent<FallingDownTree>().FallingDown(dir);

            string dirStr = (dir > 0) ? "������" : "����";

            Debug.Log(dirStr + "���� ������ PUSH !!!");
        }
        */
    }

}