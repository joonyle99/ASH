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
    [SerializeField] protected float _targetCheckCoolTime;
    [SerializeField] protected bool _isDuringCoolTime;
    public bool IsDuringCoolTime
    {
        get => _isDuringCoolTime;
        set => _isDuringCoolTime = value;
    }
    [SerializeField] protected bool _isUsable;
    public bool IsUsable
    {
        get => _isUsable;
        set => _isUsable = value;
    }

    public abstract bool IsTargetWithinRange();
    public virtual IEnumerator CoolTimeCoroutine()
    {
        IsDuringCoolTime = true;
        yield return new WaitForSeconds(_targetCheckCoolTime);
        IsDuringCoolTime = false;
    }
    public virtual void StartCoolTimeCoroutine()
    {
        StartCoroutine(CoolTimeCoroutine());
    }
}
