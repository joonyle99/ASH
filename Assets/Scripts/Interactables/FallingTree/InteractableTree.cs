using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableTree : InteractableObject
{
    public FallingTopTree FallingTopTree;

    // Own Interaction Type 세팅
    public InteractionType.Type ownInteractionType = InteractionType.Type.PUSH;

    protected override void OnInteract()
    {
        // Debug.Log("나무와의 상호작용 실행");

        // Player에게 Own Interaction Type을 넘겨준다
        SceneContext.Current.Player.GetComponent<InteractionState>().ChangeInteractionType(ownInteractionType);
    }

    public override void UpdateInteracting()
    {
        // 상호작용 종료 타이밍
        if (InputManager.Instance.InteractionKey.KeyUp || FallingTopTree.IsFalling)
        {
            // 상호작용 종료
            FallingTopTree.FinishPush();
            FinishInteraction();

            return;
        }

        // Top Tree 부분을 민다.
        float dir = Mathf.Sign(FallingTopTree.transform.position.x - SceneContext.Current.Player.transform.position.x);
        FallingTopTree.ExcutePush(dir);
    }

}