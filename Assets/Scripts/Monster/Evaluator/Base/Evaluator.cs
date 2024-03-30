using System.Collections;
using UnityEngine;

/// <summary>
/// �ǵ��� Ŭ����
/// </summary>
public abstract class Evaluator : MonoBehaviour
{
    [Header("Evaluator")]
    [Space]

    [SerializeField] protected LayerMask _targetLayer;          // �ǵ� ��� ���̾�
    [SerializeField] protected BoxCollider2D _checkCollider;    // �ǵ� �ݶ��̴�

    [Space]

    [SerializeField] private float _targetEvaluatorCoolTime;    // �ǵ� ��Ÿ��
    public float TargetCheckCoolTime
    {
        get => _targetEvaluatorCoolTime;
        set => _targetEvaluatorCoolTime = value;
    }
    [SerializeField] private bool _isDuringCoolTime;            // �ǵ��� ��Ÿ�� �� ����
    public bool IsDuringCoolTime
    {
        get => _isDuringCoolTime;
        set => _isDuringCoolTime = value;
    }
    [SerializeField] private bool _canWorking = true;           // �ǵ��� �۵� ����ġ
    public bool CanWorking
    {
        get => _canWorking;
        set => _canWorking = value;
    }
    [SerializeField] private bool _isUsable = true;             // �ǵ��� ��� ���� ����
    public bool IsUsable
    {
        get => _isUsable;
        set => _isUsable = value;
    }

    private Coroutine _coolTimeCoroutine;

    // Ŀ���� �ǵ� �̺�Ʈ ����
    protected delegate void CustomEvaluationEvent(Vector3 targetPoint);
    protected CustomEvaluationEvent customEvaluationEvent;

    public virtual Collider2D IsTargetWithinRange()
    {
        // check coolTime and usable
        if (IsDuringCoolTime || !_canWorking || !IsUsable)
            return null;

        // check target within range
        // only one collider check !
        var targetCollider = Physics2D.OverlapBox(_checkCollider.transform.position, _checkCollider.bounds.size, 0f, _targetLayer);
        if (targetCollider)
        {
            // check player
            // ...

            PlayerBehaviour player = targetCollider.GetComponent<PlayerBehaviour>();
            if (player)
            {
                if (!player.IsDead)
                {
                    // �߰� �۾�
                    if (customEvaluationEvent != null)
                    {
                        // �÷��̾��� Ÿ�� ����Ʈ ����
                        Vector3 playerTargetPoint = player.transform.position + new Vector3(0f, player.BodyCollider.bounds.extents.y * 1.5f, 0f);
                        // Debug.DrawRay(playerTargetPoint, Vector3.right, Color.yellow);

                        // �÷��̾� ��ġ�� ����
                        customEvaluationEvent(playerTargetPoint);

                        return targetCollider;
                    }
                }
            }

            // check other
            // ...

            // �߰� �۾�
            // (0, 0, 0)�� ����
            if (customEvaluationEvent != null)
                customEvaluationEvent(Vector3.zero);

            return targetCollider;
        }

        return null;
    }
    private IEnumerator CoolTimeCoroutine()
    {
        IsDuringCoolTime = true;
        yield return new WaitForSeconds(_targetEvaluatorCoolTime);
        IsDuringCoolTime = false;
    }
    public virtual void StartEvaluatorCoolTime()
    {
        if (_targetEvaluatorCoolTime < 0.01f)
        {
            Debug.LogWarning("Evaluator CoolTime�� ����ϱ� ���� _targetCheckCoolTime�� �������ּ���");
            return;
        }

        if (this._coolTimeCoroutine != null)
            StopCoroutine(_coolTimeCoroutine);

        this._coolTimeCoroutine = StartCoroutine(CoolTimeCoroutine());
    }

    public void SetCanWorking(bool canWorking)
    {
        CanWorking = canWorking;
    }
}
