using System.Collections;
using UnityEngine;

/// <summary>
/// 판독기 클래스
/// </summary>
public abstract class Evaluator : MonoBehaviour
{
    [Header("Evaluator")]
    [Space]

    [SerializeField] protected LayerMask _targetLayer;
    [SerializeField] protected BoxCollider2D _checkCollider;
    [SerializeField] private float _targetCheckCoolTime;
    [SerializeField] private bool _isDuringCoolTime;
    public bool IsDuringCoolTime
    {
        get => _isDuringCoolTime;
        set => _isDuringCoolTime = value;
    }
    [SerializeField] private bool _isUsable;
    public bool IsUsable
    {
        get => _isUsable;
        set => _isUsable = value;
    }

    private Coroutine _coolTimeCoroutine;

    public abstract bool IsTargetWithinRange();
    public virtual IEnumerator CoolTimeCoroutine()
    {
        // Debug.Log("CoolTime 시작");

        IsDuringCoolTime = true;
        yield return new WaitForSeconds(_targetCheckCoolTime);
        IsDuringCoolTime = false;

        // Debug.Log("CoolTime 끝");
    }
    public virtual void StartCoolTimeCoroutine()
    {
        if (_targetCheckCoolTime < 0.01f)
        {
            Debug.LogWarning("CheckCoolTime을 사용할거면 값을 설정해주세요 ");
            return;
        }

        if (this._coolTimeCoroutine != null)
            StopCoroutine(_coolTimeCoroutine);

        this._coolTimeCoroutine = StartCoroutine(CoolTimeCoroutine());
    }
}
