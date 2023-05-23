using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터의 행동을 전역적으로 조작하는 기반 클래스
/// 담당자 : 공준열
/// </summary>
public class MonsterBehaviour : StateMachineBase
{
    // 몬스터의 Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 피격 collision에서 "PlayerBasicAttackHitbox" 컴포넌트를 찾음
        if (collision.GetComponent<PlayerBasicAttackHitbox>() != null)
        {
            Debug.Log("Hitted by basic attack");
        }
    }
}
