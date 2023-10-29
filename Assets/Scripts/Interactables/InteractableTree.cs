using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableTree : InstantInteractableObject
{
    public GameObject topOfTree;

    public override void Interact()
    {
        Debug.Log("이것은 나무와의 상호작용 입니다. -> 이름은 : " + topOfTree.name);

        // 여기서 나무를 밀어줘야 하는데..

        // 플레이어 정보를 알 수 있나?

        // topOfTree.GetComponent<FallingDownTree>().FallingDown();
    }
}