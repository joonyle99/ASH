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

    private bool _startUsableFlag;
    [SerializeField] private bool _isUsable = true;
    public bool IsUsable
    {
        get => _isUsable;
        set => _isUsable = value;
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

    protected MonsterBehavior monster;
    private Coroutine _coolTimeCoroutine;

    // �ǵ� �̺�Ʈ ����
    protected delegate void EvaluationDelegate(Vector3 targetPoint);        // void EvaluationDelegate(Vector3 targetPoint)�̶�� ��������Ʈ(�븮��) ����
    protected event EvaluationDelegate EvaluationEvent;                     // EvaluationDelegate ��������Ʈ�� �̺�Ʈ�� ����(��������Ʈ�� �ܺο��� �ڴ�� ȣ���ϴ� ������ ����
                                                                            // event Ű����� �ܺο��� EvaluationEvent(Vector3.back); �� ���� ȣ���� �� ������ ���´�

    public virtual void Awake()
    {
        monster = GetComponent<MonsterBehavior>();
        _startUsableFlag = IsUsable;
    }
    public virtual void OnDisable()
    {
        IsUsable = _startUsableFlag;
        IsDuringCoolTime = false;
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

        // check if monster attached evaluator is dead
        if (monster.IsDead) return null;

        // check target within range (only one target)
        Collider2D targetCollider = Physics2D.OverlapBox(checkCollider.transform.position, checkCollider.bounds.size, 0f, targetLayer);
        if (targetCollider == null) return null;

        // Ž���� ����� �̸��� ���
        // Debug.Log($"{targetCollider.gameObject.name}");

        // check player
        PlayerBehaviour player = targetCollider.GetComponent<PlayerBehaviour>();
        if (player)
        {
            if (!player.IsDead)
            {
                // do additional event
                if (EvaluationEvent != null)
                {
                    // �÷��̾��� Ÿ�� ����Ʈ ����
                    Vector3 playerPos = player.transform.position + new Vector3(0f, player.BodyCollider.bounds.extents.y * 1.5f, 0f);

                    EvaluationEvent(playerPos);

                    // playerPos�� �߽������ϴ� ���ڰ� ǥ��
                    var vec1 = new Vector3(-1f, 1f, 0f);
                    var vec2 = new Vector3(1f, -1f, 0f);
                    var vec3 = new Vector3(-1f, -1f, 0f);
                    var vec4 = new Vector3(1f, 1f, 0f);
                    Debug.DrawLine(playerPos + vec1, playerPos + vec2, Color.red);
                    Debug.DrawLine(playerPos + vec3, playerPos + vec4, Color.red);
                }
            }
        }

        // return target collider
        return targetCollider;
    }

    private IEnumerator CoolTimeCoroutine()
    {
        // Debug.Log("Ÿ�̸� ����");
        IsDuringCoolTime = true;

        // ��Ÿ�� �ð���ŭ ���
        yield return new WaitForSeconds(_targetEvaluatorCoolTime);

        IsDuringCoolTime = false;
        // Debug.Log("Ÿ�̸� ����");
    }
    public virtual MonsterBehavior.ActionDelegate StartEvaluatorCoolTime()
    {
        if (_targetEvaluatorCoolTime < 0.01f)
        {
            Debug.LogWarning("Evaluator CoolTime�� ����ϱ� ���� _targetCheckCoolTime�� �������ּ���");
            return null;
        }

        if (_coolTimeCoroutine != null)
            StopCoroutine(_coolTimeCoroutine);

        _coolTimeCoroutine = StartCoroutine(CoolTimeCoroutine());

        // �ʿ��ϴٸ� �߰� �޼��带 ��ȯ
        return null;
    }
}
