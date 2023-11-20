using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D.TopDownShooter;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 플레이어가 상호작용 하는 경우에 진입하는 상태
/// Enum 정보를 가지고 애니메이션 및 동작을 적용한다
/// </summary>
public class InteractionState : PlayerState
{
    // 상호작용 중인 타겟 오브젝트
    [SerializeField] private InteractableObject _targetObject = null;

    // 현재의 Interaction Type
    [SerializeField] private InteractionType _curInteractionType = InteractionType.None;

    #region PUSH

    #endregion

    #region ROLL

    [SerializeField] private float _rollPower = 5f;
    [SerializeField] private float _rollMaxSpeed = 0.8f;

    #endregion

    protected override void OnEnter()
    {
        SetTargetObject(GetComponent<InteractionController>().InteractionTarget);
        _curInteractionType = _targetObject.InteractionTypeWithPlayer;
        // Animation Parmeter 설정
        switch (_curInteractionType)
        {
            case InteractionType.None:
                Debug.LogWarning("Interaction not set");
                break;
            case InteractionType.Push:
                Player.Animator.SetBool("IsPush", true);
                break;
            case InteractionType.Roll:
                Player.Animator.SetBool("IsPush", true);
                Rigidbody2D targetRigid = _targetObject.GetComponent<Rigidbody2D>();
                if (targetRigid == null)
                    return;
                _rollPower = targetRigid.mass * 1.1f * Player.Rigidbody.gravityScale;
                break;
        }
    }

    protected override void OnUpdate()
    {
        // Debug.Log("OnUpdate Interaction State");

        switch (_curInteractionType)
        {
            case InteractionType.None:
            case InteractionType.Push:
                break;
            case InteractionType.Roll:

                if (Mathf.Abs(Player.Rigidbody.velocity.x) > _rollMaxSpeed)
                    Player.Rigidbody.velocity = new Vector2(_rollMaxSpeed * Mathf.Sign(Player.Rigidbody.velocity.x),
                        Player.Rigidbody.velocity.y);

                break;
        }
    }

    protected override void OnFixedUpdate()
    {
        // Debug.Log("OnFixedUpdate Interaction State");

        switch (_curInteractionType)
        {
            case InteractionType.None:
            case InteractionType.Push:
                break;
            case InteractionType.Roll:

                // 오브젝트가 아닌 플레이어 이동에 힘을 적용시킨다
                // 상호작용 중인 오브젝트의 무게까지 합한 이동값 계산
                // 플레이어의 GravitySclae이 1이 아닌 5로 설정되어있다. 이 값을 고려해서 힘을 줘야한다.

                Player.Rigidbody.AddForce(Vector2.right * _rollPower * Player.RawInputs.Movement.x);

                break;
        }
    }

    protected override void OnExit()
    {
        // Animation Parmeter 초기화
        switch (_curInteractionType)
        {
            case InteractionType.None:
                break;
            case InteractionType.Push:
            case InteractionType.Roll:
                Player.Animator.SetBool("IsPush", false);
                break;
        }

        Finalize();
    }

    public void SetTargetObject(InteractableObject targetObject)
    {
        if (targetObject == null)
        {
            Debug.Log("Target Object가 없습니다");
            return;
        }

        _targetObject = targetObject;

        return;
    }

    public void Finalize()
    {
        _targetObject = null;
        _curInteractionType = InteractionType.None;
        _rollPower = 0f;

        return;
    }
}
