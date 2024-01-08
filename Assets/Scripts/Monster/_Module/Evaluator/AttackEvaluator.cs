using System.Collections;
using UnityEngine;

/// <summary>
/// Attack 모듈
/// Monster에 붙히는 용도로 사용
/// </summary>
public class AttackEvaluator : MonoBehaviour
{
    [Header("Attack Evaluator")]
    [Space]

    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private Vector2 _attackBoxSize;
    [SerializeField] private float _targetWaitTime = 3f;
    [SerializeField] private bool _isAttackable = true;

    public bool IsTargetWithinAttackRange()
    {
        if (!_isAttackable)
            return false;

        // 탐지 범위 안에 들어왔는지 확인
        Collider2D targetCollider = Physics2D.OverlapBox(transform.position, _attackBoxSize, 0f, _targetLayer);
        if (targetCollider != null)
        {
            // 플레이어인지 확인
            PlayerBehaviour player = targetCollider.GetComponent<PlayerBehaviour>();
            if (player != null && !player.IsDead)
                return true;
        }

        return false;
    }

    private IEnumerator AttackableTimer()
    {
        _isAttackable = false;
        yield return new WaitForSeconds(_targetWaitTime);
        _isAttackable = true;
    }

    public void StartAttackableTimer()
    {
        StartCoroutine(AttackableTimer());
    }

    private void OnDrawGizmosSelected()
    {
        // 공격 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.transform.position, _attackBoxSize);
    }
}
