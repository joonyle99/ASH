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
    }

}