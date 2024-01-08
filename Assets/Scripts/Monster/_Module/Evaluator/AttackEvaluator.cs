using System.Collections;
using UnityEngine;

/// <summary>
/// Attack ���
/// Monster�� ������ �뵵�� ���
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

        // Ž�� ���� �ȿ� ���Դ��� Ȯ��
        Collider2D targetCollider = Physics2D.OverlapBox(transform.position, _attackBoxSize, 0f, _targetLayer);
        if (targetCollider != null)
        {
            // �÷��̾����� Ȯ��
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
        // ���� ����
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.transform.position, _attackBoxSize);
    }
}
