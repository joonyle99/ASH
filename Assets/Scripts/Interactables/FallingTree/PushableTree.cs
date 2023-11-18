using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PushableTree : InteractableObject
{
    public FallingTreeByPush FallingTreeByPush;

    // Own Interaction Type 세팅
    public InteractionType.Type ownInteractionType = InteractionType.Type.PUSH;

    protected override void OnInteract()
    {
        // Debug.Log("나무와의 상호작용 실행");

        // Player에게 Own Interaction Type을 넘겨준다
        SceneContext.Current.Player.GetComponent<InteractionState>().SetInteractionType(ownInteractionType);
    }

    public override void UpdateInteracting()
    {
        // 상호작용 종료 타이밍
        if (InputManager.Instance.InteractionKey.KeyUp || FallingTreeByPush.IsFalling)
        {
            // 상호작용 종료
            FallingTreeByPush.FinishPush();
            FinishInteraction();

            // 더이상 상호작용 못하게 막는다
            IsInteractable = false;

            return;
        }

        // Top Tree 부분을 민다.
        float dir = Mathf.Sign(FallingTreeByPush.transform.position.x - SceneContext.Current.Player.transform.position.x);
        FallingTreeByPush.ExcutePush(dir);
    }
}