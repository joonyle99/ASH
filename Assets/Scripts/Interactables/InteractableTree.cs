using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableTree : InteractableObject
{
    public TopOfTree topOfTree;

    protected override void OnInteract()
    {
        Debug.Log("나무와의 상호작용 실행");

        // 플레이어를 상호작용 : Push State로
        // SceneContext.Current.Player.GetComponent<PlayerBehaviour>().ChangeState<PushState>();
    }
    public override void UpdateInteracting()
    {
        // 종료 사인을 줘야한다
        if (InputManager.Instance.InteractionKey.KeyUp)
        {
            topOfTree.FinishPush();
            FinishInteraction();

            return;
        }

        // Top Tree 부분을 민다.
        float dir = Mathf.Sign(topOfTree.transform.position.x - SceneContext.Current.Player.transform.position.x);
        topOfTree.ExcutePush(dir);
    }

}