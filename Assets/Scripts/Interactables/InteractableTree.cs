using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableTree : InstantInteractableObject
{
    public GameObject topOfTree;

    public override void Interact()
    {
        Debug.Log("�̰��� �������� ��ȣ�ۿ� �Դϴ�. -> �̸��� : " + topOfTree.name);

        // ���⼭ ������ �о���� �ϴµ�..

        // �÷��̾� ������ �� �� �ֳ�?

        // topOfTree.GetComponent<FallingDownTree>().FallingDown();
    }
}