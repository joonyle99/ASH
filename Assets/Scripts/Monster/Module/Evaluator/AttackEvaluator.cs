using System.Collections;
using UnityEngine;

public class AttackEvaluator : MonoBehaviour
{
    [Header("Attack Evaluator")]
    [Space]

    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private BoxCollider2D _attackCheckBoxCollider;
    [SerializeField] private float _targetWaitTime = 5f;
    [SerializeField] private bool _isAttackable = true;
    private Vector2 _attackCheckBoxSize;

    private void Awake()
    {
        _attackCheckBoxSize = _attackCheckBoxCollider.bounds.size;
    }

    public bool IsTargetWithinAttackRange()
    {
        if (!_isAttackable)
            return false;

        // 탐지 범위 안에 들어왔는지 확인
        Collider2D targetCollider = Physics2D.OverlapBox(_attackCheckBoxCollider.transform.position, _attackCheckBoxSize, 0f, _targetLayer);
        if (targetCollider)
        {
            // 플레이어인지 확인
            PlayerBehaviour player = targetCollider.GetComponent<PlayerBehaviour>();
            if (player && !player.IsDead)
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
}
