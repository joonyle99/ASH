using UnityEngine;

public class AttackEvaluator : MonoBehaviour
{
    [Header("Attack Evaluator")]
    [Space]

    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private Vector2 _attackBoxSize;
    [SerializeField] private float _targetWaitTime = 1.5f;
    [SerializeField] private float _elapsedWaitTime;
    [SerializeField] private bool _isAttackable;

    public LayerMask TargetLayer
    {
        get { return _targetLayer; }
        set { _targetLayer = value; }
    }

    public Vector2 AttackBoxSize
    {
        get { return _attackBoxSize; }
        set { _attackBoxSize = value; }
    }

    public bool IsAttackable
    {
        get { return _isAttackable; }
        set { _isAttackable = value; }
    }

    void Update()
    {
        if (!_isAttackable)
        {
            _elapsedWaitTime += Time.deltaTime;

            if (_elapsedWaitTime > _targetWaitTime)
            {
                _elapsedWaitTime = 0f;
                _isAttackable = true;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 공격 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.transform.position, _attackBoxSize);
    }
}
