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

        if (Mathf.Abs(inputState.Horizontal) < 0.1f)
        {
            // -------------------------------------------- //
            //      나무를 밀지 않고 가만히 있는 코드         //
            // -------------------------------------------- //

            // Debug.Log("나무를 가만히 잡고만 있는다");

            FallingTreeByPush.StopPush();
        }
        else
        {

            // -------------------------------------------- //
            //      실제로 플레이어가 나무를 미는 코드         //
            // -------------------------------------------- //

            // Debug.Log("나무를 민다");

            // Push Dir 설정
            float pushDir = Mathf.Sign(FallingTreeByPush.transform.position.x - SceneContext.Current.Player.transform.position.x);

            if (pushDir > 0f)
            {
                if (inputState.Horizontal > 0f)
                {
                    // Top Tree 부분을 민다.
                    FallingTreeByPush.StartPush(pushDir);
                }
            }
            else if (pushDir < 0f)
            {
                if (inputState.Horizontal < 0f)
                {
                    FallingTreeByPush.StartPush(pushDir);
                }
            }
        }
    }
}