using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PushableTree : InteractableObject
{
    public FallingTreeByPush FallingTreeByPush;

    protected override void OnInteract()
    {
        // Debug.Log("나무와의 상호작용 실행");

    }

    public override void UpdateInteracting()
    {
        InputState inputState = InputManager.Instance.GetState();

        // 상호작용 종료 타이밍
        if (InputManager.Instance.InteractionKey.KeyUp || inputState.Horizontal < 0.1f || FallingTreeByPush.IsFalling)
        {
            // 상호작용 해제 타이밍
            if (FallingTreeByPush.IsFalling)
            {
                // 더이상 상호작용 못하게 막는다
                IsInteractable = false;
            }

            // 상호작용 종료
            FallingTreeByPush.FinishPush();
            FinishInteraction();

            return;
        }

        // Push Dir 설정
        float pushDir = Mathf.Sign(FallingTreeByPush.transform.position.x - SceneContext.Current.Player.transform.position.x);

        // 오른쪽으로 밀어야하는 경우
        if (pushDir > 0)
        {
            // 오른쪽 방향키를 누른다
            if (inputState.Horizontal > 0f)
            {
                // Top Tree 부분을 민다.
                FallingTreeByPush.ExcutePush(pushDir);
            }
        }
        // 왼쪽으로 밀어야하는 경우
        else if (pushDir < 0)
        {
            // 왼쪽 방향키를 누른다
            if (inputState.Horizontal < 0f)
            {
                // Top Tree 부분을 민다.
                FallingTreeByPush.ExcutePush(pushDir);
            }
        }
    }
}