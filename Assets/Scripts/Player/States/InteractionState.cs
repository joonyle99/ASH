using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D.TopDownShooter;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

/// <summary>
/// 플레이어가 상호작용 하는 경우에 진입하는 상태
/// Enum 정보를 가지고 애니메이션 및 동작을 적용한다
/// </summary>
public class InteractionState : PlayerState
{
    // 상호작용 중인 타겟 오브젝트
    [SerializeField] private InteractableObject _targetObject = null;

    // 현재의 Interaction MonsterType
    [SerializeField] private InteractionType _curInteractionType = InteractionType.None;

    #region PUSH

    #endregion

    #region ROLL

    [SerializeField] private float _rollingPower = 5f;
    [SerializeField] private float _rollingMaxSpeed = 0.8f;

    #endregion

    protected override void OnEnter()
    {
        // Debug.Log("OnEnter InteractionState");

        Player.Animator.SetTrigger("Interact");

        // 타겟 오브젝트와 상호작용 타입 설정
        SetTargetObject(this.GetComponent<InteractionController>().InteractionTarget);
        SetInteractionType(_targetObject.InteractionTypeWithPlayer);

        // 애니메이션 파라미터 설정
        switch (_curInteractionType)
        {
            case InteractionType.Push:
                Player.Animator.SetBool("IsPush", true);
                break;
            case InteractionType.Roll:
                Player.Animator.SetBool("IsPush", true);

                // rollingPower 설정
                Rigidbody2D targetRigid = _targetObject.GetComponent<Rigidbody2D>();

                if (targetRigid == null)
                    return;

                // 플레이어의 GravitySclae이 1이 아닌 5로 설정되어있다. 이 값을 고려해서 힘을 줘야한다.
                //_rollingPower = targetRigid.mass * 1.1f * Player.Rigidbody.gravityScale;

                break;
        }
    }

    /// <summary>
    /// Game Logic
    /// </summary>
    protected override void OnUpdate()
    {
        // Debug.Log("OnUpdate Interaction State");
        return;
        switch (_curInteractionType)
        {
            case InteractionType.Push:
                break;
            case InteractionType.Roll:

                if (Mathf.Abs(Player.Rigidbody.velocity.x) > _rollingMaxSpeed)
                    Player.Rigidbody.velocity = new Vector2(_rollingMaxSpeed * Mathf.Sign(Player.Rigidbody.velocity.x),
                        Player.Rigidbody.velocity.y);

                break;
        }
    }
    /// <summary>
    /// Physics
    /// </summary>
    protected override void OnFixedUpdate()
    {
        // Debug.Log("OnFixedUpdate Interaction State");


        switch (_curInteractionType)
        {
            case InteractionType.Push:
                break;
            case InteractionType.Roll:

                // 오브젝트가 아닌 플레이어 이동에 힘을 적용시킨다
                // 상호작용 중인 오브젝트의 무게까지 합한 이동값 계산
                Player.Rigidbody.AddForce(Player.RawInputs.Movement * _rollingPower);
                break;
        }
    }

    protected override void OnExit()
    {
        // Debug.Log("OnExit InteractionState");

        // 애니메이션 파라미터 초기화
        switch (_curInteractionType)
        {
            case InteractionType.Push:
            case InteractionType.Roll:
                Player.Animator.SetBool("IsPush", false);
                break;
        }

        FinalizeInteraction();
    }

    public void SetTargetObject(InteractableObject targetObject)
    {
        if (targetObject == null)
        {
            Debug.Log("Target Object가 없습니다");
            return;
        }

        _targetObject = targetObject;
    }

    public void SetInteractionType(InteractionType interactionType)
    {
        _curInteractionType = interactionType;

        if (interactionType == InteractionType.None)
            Debug.LogWarning("Interaction not set");
    }

    public void FinalizeInteraction()
    {
        _targetObject = null;
        _curInteractionType = InteractionType.None;
    }
}
