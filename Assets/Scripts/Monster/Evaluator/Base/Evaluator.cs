using System.Collections;
using UnityEngine;

/// <summary>
/// 범위 안에 대상이 들어왔는 지를 판단하는 '판독기' 클래스
/// </summary>
public abstract class Evaluator : MonoBehaviour
{
    [Header("─────── Evaluator ───────")]
    [Space]

    [SerializeField] protected LayerMask targetLayer;           // 판독 대상 레이어
    [SerializeField] protected BoxCollider2D checkCollider;     // 판독 콜라이더

    [Header("____ Togle Options ____")]
    [Space]

    [Tooltip("판독기를 사용할지 결정합니다")]
    [SerializeField] private bool _isUsable = true;
    public bool IsUsable
    {
        get => _isUsable;
        set => _isUsable = value;
    }

    [Header("____ CoolTime ____")]
    [Space]

    [SerializeField] private bool _isWaitingEvent;              // 이벤트 대기 여부
    public bool IsWaitingEvent
    {
        get => _isWaitingEvent;
        set => _isWaitingEvent = value;
    }
    [SerializeField] private bool _isDuringCoolTime;            // 판독기 쿨타임 여부
    public bool IsDuringCoolTime
    {
        get => _isDuringCoolTime;
        set => _isDuringCoolTime = value;
    }
    [SerializeField] private float _targetEvaluatorCoolTime;    // 판독 쿨타임 시간
    public float TargetCheckCoolTime
    {
        get => _targetEvaluatorCoolTime;
        set => _targetEvaluatorCoolTime = value;
    }

    private bool _startUsableFlag;

    protected MonsterBehaviour monster;
    private Coroutine _coolTimeCoroutine;
    private Coroutine _waitCoroutine;

    // 판독 필터 이벤트
    public delegate bool FilterDelegate(Vector3 targetPoint);
    public event FilterDelegate FilterEvent;

    // 판독 이벤트 정의
    protected delegate void EvaluationDelegate(Vector3 targetPoint);        // void EvaluationDelegate(Vector3 targetPoint)이라는 델리게이트(대리자) 선언
    protected event EvaluationDelegate EvaluationEvent;                     // EvaluationDelegate 델리게이트를 이벤트로 선언(델리게이트를 외부에서 멋대로 호출하는 문제를 방지
                                                                            // event 키워드는 외부에서 EvaluationEvent(Vector3.back); 와 같이 호출할 수 없도록 막는다

    public delegate IEnumerator WaitEventDelegate();
    public event WaitEventDelegate WaitEvent;

    public virtual void Awake()
    {
        monster = GetComponent<MonsterBehaviour>();
        _startUsableFlag = IsUsable;
    }
    public virtual void OnDisable()
    {
        IsUsable = _startUsableFlag;
        IsDuringCoolTime = false;
        IsWaitingEvent = false;
    }

    /// <summary>
    /// 판독 콜라이더의 범위 안에 들어오는 타겟을 감지한다
    /// </summary>
    /// <returns></returns>
    public virtual Collider2D IsTargetWithinRange()
    {
        // check coolTime and usable
        if (!IsUsable || IsDuringCoolTime || IsWaitingEvent)
            return null;

        // check if monster attached evaluator is dead
        if (monster.IsDead) return null;

        // check target within range (only one target)
        Collider2D targetCollider = Physics2D.OverlapBox(checkCollider.transform.position, checkCollider.bounds.size, 0f, targetLayer);
        if (targetCollider == null) return null;

        // 탐지한 대상의 이름을 출력
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
                    // 플레이어의 타겟 포인트 설정
                    Vector3 playerPos = player.transform.position + new Vector3(0f, player.BodyCollider.bounds.extents.y * 1.5f, 0f);

                    // 필터링 조건을 만족하면 판독 이벤트를 발생시키지 않음
                    bool isFiltered = false;
                    if (FilterEvent != null)
                    {
                        // Debug.Log("판독기 필터링 검사 시작");
                        isFiltered = FilterEvent(playerPos);
                    }
                    if (isFiltered) return null;

                    EvaluationEvent(playerPos);

                    // playerPos을 중심으로하는 십자가 표시
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

    protected virtual IEnumerator CoolTimeCoroutine()
    {
        bool isNeverBoth = true;

        // 특정 이벤트가 등록되어 있다면 실행
        if (WaitEvent != null)
        {
            isNeverBoth = false;

            // Debug.Log("이벤트 대기 시작");
            IsWaitingEvent = true;

            yield return WaitEvent();

            IsWaitingEvent = false;
            // Debug.Log("이벤트 대기 끝");
        }

        // 쿨타임이 0.01초 이상이면 쿨타임 실행
        if (_targetEvaluatorCoolTime > 0.01f)
        {
            isNeverBoth = false;

            //Debug.Log("쿨타임 대기 시작");
            IsDuringCoolTime = true;

            yield return new WaitForSeconds(_targetEvaluatorCoolTime);

            IsDuringCoolTime = false;
            //Debug.Log("쿨타임 대기 끝");
        }

        if (isNeverBoth)
        {
            Debug.Log($"쿨타임이 실행되었지만, 어떠한 이벤트나 대기 시간을 기다리지 않습니다");
            yield return null;
        }
    }
    public MonsterBehaviour.ActionDelegate StartCoolTime()
    {
        if (_coolTimeCoroutine != null)
            StopCoroutine(_coolTimeCoroutine);

        _coolTimeCoroutine = StartCoroutine(CoolTimeCoroutine());

        // 필요하다면 추가 메서드를 반환
        return null;
    }

    public IEnumerator WaitForRespawn()
    {
        if (!_startUsableFlag) yield break;

        IsUsable = false;

        yield return new WaitUntil(() => monster.IsRespawn);

        IsUsable = true;
    }
}
