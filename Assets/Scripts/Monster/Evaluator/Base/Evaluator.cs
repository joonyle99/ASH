using System.Collections;
using UnityEngine;

/// <summary>
/// 판독기 클래스
/// </summary>
public abstract class Evaluator : MonoBehaviour
{
    [Header("Evaluator")]
    [Space]

    [SerializeField] protected LayerMask _targetLayer;          // 판독 대상 레이어
    [SerializeField] protected BoxCollider2D _checkCollider;    // 판독 콜라이더

    [Space]

    [SerializeField] private float _targetEvaluatorCoolTime;    // 판독 쿨타임
    public float TargetCheckCoolTime
    {
        get => _targetEvaluatorCoolTime;
        set => _targetEvaluatorCoolTime = value;
    }
    [SerializeField] private bool _isDuringCoolTime;            // 판독기 쿨타임 중 여부
    public bool IsDuringCoolTime
    {
        get => _isDuringCoolTime;
        set => _isDuringCoolTime = value;
    }
    [SerializeField] private bool _canWorking = true;           // 판독기 작동 스위치
    public bool CanWorking
    {
        get => _canWorking;
        set => _canWorking = value;
    }
    [SerializeField] private bool _isUsable = true;             // 판독기 사용 가능 여부
    public bool IsUsable
    {
        get => _isUsable;
        set => _isUsable = value;
    }

    private Coroutine _coolTimeCoroutine;

    // 커스텀 판독 이벤트 정의
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
                    // 추가 작업
                    if (customEvaluationEvent != null)
                    {
                        // 플레이어의 타겟 포인트 설정
                        Vector3 playerTargetPoint = player.transform.position + new Vector3(0f, player.BodyCollider.bounds.extents.y * 1.5f, 0f);
                        // Debug.DrawRay(playerTargetPoint, Vector3.right, Color.yellow);

                        // 플레이어 위치를 전달
                        customEvaluationEvent(playerTargetPoint);

                        return targetCollider;
                    }
                }
            }

            // check other
            // ...

            // 추가 작업
            // (0, 0, 0)을 전달
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
            Debug.LogWarning("Evaluator CoolTime을 사용하기 위해 _targetCheckCoolTime을 설정해주세요");
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
