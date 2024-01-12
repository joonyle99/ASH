using UnityEngine;

public class FloatingChaseEvaluator : Evaluator
{
    [Header("Floating Chase Evaluator")]
    [Space]

    // Test Code
    [SerializeField] private GameObject checkPrefab;
    private GameObject _chaseTargetPoint;

    public Transform TargetTrans { get; private set; }

    public override bool IsTargetWithinRange()
    {
        if (IsDuringCoolTime || !IsUsable)
            return false;

        // Detect Collider
        Collider2D targetCollider = Physics2D.OverlapBox(_checkCollider.transform.position, _checkCollider.bounds.size, 0f, _targetLayer);
        if (targetCollider)
        {
            // Check Player
            PlayerBehaviour player = targetCollider.GetComponent<PlayerBehaviour>();
            if (player && !player.IsDead)
            {
                // Set Destination
                SetTargetTrans(player.transform);

                // Create Debug Object
                if (!_chaseTargetPoint)
                {
                    _chaseTargetPoint = Instantiate(checkPrefab, TargetTrans.position, Quaternion.identity, transform.parent);
                    _chaseTargetPoint.name = "Chase Target Point";
                }
                else
                    _chaseTargetPoint.transform.position = player.transform.position;

                return true;
            }
        }

        // Delete Debug Object
        if (_chaseTargetPoint)
            Destroy(_chaseTargetPoint);

        return false;
    }

    public void SetTargetTrans(Transform trans)
    {
        TargetTrans = trans;
    }
}
