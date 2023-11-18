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
    [SerializeField] private InteractionType.Type _curInteractionType = InteractionType.Type.NULL;

    #region PUSH

    #endregion

    #region ROLL

    [SerializeField] private float _rollPower = 5f;
    [SerializeField] private float _rollMaxSpeed = 0.8f;

    #endregion

    protected override void OnEnter()
    {
        Debug.Log("Enter Interaction State");

        // Target Object 가져오기
        SetTargetObject(this.GetComponent<InteractionController>().GetTargetObject());

        // Animation Parmeter 설정
        switch (_curInteractionType)
        {
            case InteractionType.Type.NULL:
                Debug.LogError("상호작용 타입이 입력되지 않았습니다");
                break;
            case InteractionType.Type.PUSH:
            case InteractionType.Type.ROLL:
                Rigidbody2D targetRigid = _targetObject.GetComponent<Rigidbody2D>();
                if (targetRigid == null)
                    return;
                _rollPower = targetRigid.mass * 1.1f * Player.Rigidbody.gravityScale;
                Player.Animator.SetBool("IsPush", true);
                break;
        }
    }

    protected override void OnUpdate()
    {
        // Debug.Log("OnUpdate Interaction State");

        switch (_curInteractionType)
        {
            case InteractionType.Type.NULL:
            case InteractionType.Type.PUSH:
                break;
            case InteractionType.Type.ROLL:

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
            case InteractionType.Type.NULL:
            case InteractionType.Type.PUSH:
                break;
            case InteractionType.Type.ROLL:

                // 오브젝트가 아닌 플레이어 이동에 힘을 적용시킨다
                // 상호작용 중인 오브젝트의 무게까지 합한 이동값 계산
                // 플레이어의 GravitySclae이 1이 아닌 5로 설정되어있다. 이 값을 고려해서 힘을 줘야한다.

                Player.Rigidbody.AddForce(Vector2.right * _rollPower * Player.RawInputs.Movement.x);

                break;
        }
    }

    protected override void OnExit()
    {
        Debug.Log("OnExit Interaction State");

        // Animation Parmeter 초기화
        switch (_curInteractionType)
        {
            case InteractionType.Type.NULL:
                break;
            case InteractionType.Type.PUSH:
            case InteractionType.Type.ROLL:
                Player.Animator.SetBool("IsPush", false);
                break;
        }

        Finalize();
    }

    public void SetInteractionType(InteractionType.Type type)
    {
        _curInteractionType = type;
        return;
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
        _curInteractionType = InteractionType.Type.NULL;
        _rollPower = 0f;

        return;
    }
}
