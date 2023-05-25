using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEventPasser : MonoBehaviour
{
    PlayerAttackController _attackState;

    private void Awake()
    {
        _attackState = GetComponentInParent<PlayerAttackController>();
    }
    public void FinishBaseAttackAnim()
    {
        // IdleState·Î º¯°æ
        _attackState.AnimEvent_FinishBaseAttackAnim();
    }
}
