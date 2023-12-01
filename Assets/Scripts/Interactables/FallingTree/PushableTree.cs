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
        if (InputManager.Instance.InteractionKey.KeyUp || FallingTreeByPush.IsFalling)
        {
            // 상호작용 해제 타이밍 (나무가 이미 쓰러진 경우)
            if (FallingTreeByPush.IsFalling)
            {
                // 더이상 상호작용 못하게 막는다
                IsInteractable = false;
            }

            // 상호작용 종료
            FallingTreeByPush.StopPush();
            FinishInteraction();

            return;
        }

        float pushDir = Mathf.Sign(FallingTreeByPush.transform.position.x - SceneContext.Current.Player.transform.position.x);
        bool isSync = Mathf.Abs(pushDir - inputState.Horizontal) < 0.01f;

        if (!isSync)
            FallingTreeByPush.StopPush();
        else
            FallingTreeByPush.StartPush(pushDir);
    }
}