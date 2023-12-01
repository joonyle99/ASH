using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PushableTree : InteractableObject
{
    public FallingTreeByPush FallingTreeByPush;

    protected override void OnInteract()
    {

    }

    public override void UpdateInteracting()
    {
        InputState inputState = InputManager.Instance.GetState();

        // 상호작용 종료 타이밍
        if (InputManager.Instance.InteractionKey.KeyUp || FallingTreeByPush.IsFallingEnd)
        {
            // 나무가 이미 쓰러진 경우
            if (FallingTreeByPush.IsFallingEnd)
            {
                // 더이상 상호작용 못하게 막는다
                IsInteractable = false;
            }

            // 상호작용 종료
            FallingTreeByPush.StopPush();
            FinishInteraction();

            return;
        }

        // 플레이어가 왼쪽이면 + 플레이어가 오른쪽이면 -
        float pushDir = Mathf.Sign(FallingTreeByPush.transform.position.x - SceneContext.Current.Player.transform.position.x);

        // 같은 방향인지 계산
        bool isSyncDir = Mathf.Abs(pushDir - inputState.Horizontal) < 0.01f;

        // 같은 방향인 경우에만 민다.
        if (isSyncDir)
            FallingTreeByPush.StartPush(pushDir);
        else
            FallingTreeByPush.StopPush();

    }
}