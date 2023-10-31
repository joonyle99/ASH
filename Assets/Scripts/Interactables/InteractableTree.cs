using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableTree : InteractableObject
{
    public GameObject topOfTree;

    protected override void OnInteract()
    {
        Debug.Log("이것은 나무와의 상호작용 입니다. -> 이름은 : " + topOfTree.name);

        // 여기서 나무를 밀어줘야 하는데..

        // 플레이어 정보를 알 수 있나?

        // topOfTree.GetComponent<FallingDownTree>().FallingDown();
    }
    public override void UpdateInteracting()
    {
        throw new System.NotImplementedException();
        /*
         * InteractableObject.Update에 있던 코드
         * 
        // 쓰러지는 나무와의 상호작용
        if (Input.GetKey(_interactionKey))
        {
            GameObject topTree = (_interactionTarget as InteractableTree).topOfTree;

            float dir = Mathf.Sign(topTree.transform.position.x - this.transform.position.x);

            topTree.GetComponent<FallingDownTree>().FallingDown(dir);

            string dirStr = (dir > 0) ? "오른쪽" : "왼쪽";

            Debug.Log(dirStr + "으로 나무를 PUSH !!!");
        }
        */
    }

}