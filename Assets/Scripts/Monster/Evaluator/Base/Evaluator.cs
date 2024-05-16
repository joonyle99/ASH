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

    private bool _startUsableFlag;
    [SerializeField] private bool _isUsable = true;
    public bool IsUsable
    {
        get => _isUsable;
        set => _isUsable = value;
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

    protected MonsterBehavior monster;
    private Coroutine _coolTimeCoroutine;

    // 판독 이벤트 정의
    protected delegate void EvaluationDelegate(Vector3 targetPoint);        // void EvaluationDelegate(Vector3 targetPoint)이라는 델리게이트(대리자) 선언
    protected event EvaluationDelegate EvaluationEvent;                     // EvaluationDelegate 델리게이트를 이벤트로 선언(델리게이트를 외부에서 멋대로 호출하는 문제를 방지
                                                                            // event 키워드는 외부에서 EvaluationEvent(Vector3.back); 와 같이 호출할 수 없도록 막는다

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
    /// 판독 콜라이더의 범위 안에 들어오는 타겟을 감지한다
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

    private IEnumerator CoolTimeCoroutine()
    {
        // Debug.Log("타이머 시작");
        IsDuringCoolTime = true;

        // 쿨타임 시간만큼 대기
        yield return new WaitForSeconds(_targetEvaluatorCoolTime);

        IsDuringCoolTime = false;
        // Debug.Log("타이머 종료");
    }
    public virtual MonsterBehavior.ActionDelegate StartEvaluatorCoolTime()
    {
        if (_targetEvaluatorCoolTime < 0.01f)
        {
            Debug.LogWarning("Evaluator CoolTime을 사용하기 위해 _targetCheckCoolTime을 설정해주세요");
            return null;
        }

        if (_coolTimeCoroutine != null)
            StopCoroutine(_coolTimeCoroutine);

        _coolTimeCoroutine = StartCoroutine(CoolTimeCoroutine());

        // 필요하다면 추가 메서드를 반환
        return null;
    }
}
