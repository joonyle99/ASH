using System.Collections;
using UnityEngine;

/// <summary>
/// 범위 안에 대상이 들어왔는 지를 판단하는 '판독기' 클래스
/// </summary>
public abstract class Evaluator : MonoBehaviour
{
    [Header("Evaluator")]
    [Space]

    [SerializeField] protected LayerMask targetLayer;           // 판독 대상 레이어
    [SerializeField] protected BoxCollider2D checkCollider;     // 판독 콜라이더

    [Space]

    [SerializeField] private bool _isUsable = true;
    public bool IsUsable
    {
        get => _isUsable;
        set
        {
            _isUsable = value;

            if (!_isUsable)
                Debug.Log($"{this.name} 판독기는 비활성화 되었습니다");
        }
    }
    [SerializeField] private float _targetEvaluatorCoolTime;    // 판독 쿨타임 시간
    public float TargetCheckCoolTime
    {
        get => _targetEvaluatorCoolTime;
        set => _targetEvaluatorCoolTime = value;
    }
    [SerializeField] private bool _isDuringCoolTime;            // 판독기 쿨타임 여부
    public bool IsDuringCoolTime
    {
        get => _isDuringCoolTime;
        set => _isDuringCoolTime = value;
    }

    protected MonsterBehavior monsterBehavior;
    private Coroutine _coolTimeCoroutine;

    // 커스텀 판독 이벤트 정의
    protected delegate void EvaluationEvent(Vector3 targetPoint);       // void EvaluationEvent(Vector3 targetPoint)이라는 델리게이트(대리자) 선언
    protected event EvaluationEvent evaluationEvent;                    // EvaluationEvent 델리게이트를 이벤트로 선언(델리게이트를 외부에서 멋대로 호출하는 문제를 방지
                                                                        // event 키워드는 외부에서 evaluationEvent(Vector3.back); 와 같이 호출할 수 없도록 막는다

    public virtual void Awake()
    {
        monsterBehavior = GetComponent<MonsterBehavior>();
    }

    /// <summary>
    /// 판독 콜라이더의 범위 안에 들어오는 타겟을 감지한다
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
                    // 플레이어의 타겟 포인트 설정
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

        // 쿨타임 시간만큼 대기
        yield return new WaitForSeconds(_targetEvaluatorCoolTime);

        IsDuringCoolTime = false;
    }
    public virtual void StartEvaluatorCoolTime()
    {
        if (_targetEvaluatorCoolTime < 0.01f)
        {
            Debug.LogWarning("Evaluator CoolTime을 사용하기 위해 _targetCheckCoolTime을 설정해주세요");
            return;
        }

        if (_coolTimeCoroutine != null)
            StopCoroutine(_coolTimeCoroutine);

        _coolTimeCoroutine = StartCoroutine(CoolTimeCoroutine());
    }
}
