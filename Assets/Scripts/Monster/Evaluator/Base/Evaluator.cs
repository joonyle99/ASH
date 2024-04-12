using System.Collections;
using UnityEngine;

/// <summary>
/// ���� �ȿ� ����� ���Դ� ���� �Ǵ��ϴ� '�ǵ���' Ŭ����
/// </summary>
public abstract class Evaluator : MonoBehaviour
{
    [Header("Evaluator")]
    [Space]

    [SerializeField] protected LayerMask targetLayer;           // �ǵ� ��� ���̾�
    [SerializeField] protected BoxCollider2D checkCollider;     // �ǵ� �ݶ��̴�

    [Space]

    [SerializeField] private bool _isUsable = true;
    public bool IsUsable
    {
        get => _isUsable;
        set
        {
            _isUsable = value;

            if (!_isUsable)
                Debug.Log($"{this.name} �ǵ���� ��Ȱ��ȭ �Ǿ����ϴ�");
        }
    }
    [SerializeField] private float _targetEvaluatorCoolTime;    // �ǵ� ��Ÿ�� �ð�
    public float TargetCheckCoolTime
    {
        get => _targetEvaluatorCoolTime;
        set => _targetEvaluatorCoolTime = value;
    }
    [SerializeField] private bool _isDuringCoolTime;            // �ǵ��� ��Ÿ�� ����
    public bool IsDuringCoolTime
    {
        get => _isDuringCoolTime;
        set => _isDuringCoolTime = value;
    }

    protected MonsterBehavior monsterBehavior;
    private Coroutine _coolTimeCoroutine;

    // Ŀ���� �ǵ� �̺�Ʈ ����
    protected delegate void EvaluationEvent(Vector3 targetPoint);       // void EvaluationEvent(Vector3 targetPoint)�̶�� ��������Ʈ(�븮��) ����
    protected event EvaluationEvent evaluationEvent;                    // EvaluationEvent ��������Ʈ�� �̺�Ʈ�� ����(��������Ʈ�� �ܺο��� �ڴ�� ȣ���ϴ� ������ ����
                                                                        // event Ű����� �ܺο��� evaluationEvent(Vector3.back); �� ���� ȣ���� �� ������ ���´�

    public virtual void Awake()
    {
        monsterBehavior = GetComponent<MonsterBehavior>();
    }

    /// <summary>
    /// �ǵ� �ݶ��̴��� ���� �ȿ� ������ Ÿ���� �����Ѵ�
    /// </summary>
    /// <returns></returns>
    public virtual Collider2D IsTargetWithinRange()
    {
        // check coolTime and usable
        if (IsDuringCoolTime || !IsUsable)
            return null;

        // check target within range
        Collider2D targetCollider = Physics2D.OverlapBox(checkCollider.transform.position, checkCollider.bounds.size, 0f, targetLayer);
        if (targetCollider == null) return null;

        // check player
        PlayerBehaviour player = targetCollider.GetComponent<PlayerBehaviour>();
        if (player)
        {
            if (!player.IsDead)
            {
                // do additional event
                if (evaluationEvent != null)
                {
                    // �÷��̾��� Ÿ�� ����Ʈ ����
                    Vector3 playerPos = player.transform.position + new Vector3(0f, player.BodyCollider.bounds.extents.y * 1.5f, 0f);
                    evaluationEvent(playerPos);
                }
            }
        }

        // return target collider
        return targetCollider;
    }

    private IEnumerator CoolTimeCoroutine()
    {
        IsDuringCoolTime = true;

        // ��Ÿ�� �ð���ŭ ���
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

        if (_coolTimeCoroutine != null)
            StopCoroutine(_coolTimeCoroutine);

        _coolTimeCoroutine = StartCoroutine(CoolTimeCoroutine());
    }
}
