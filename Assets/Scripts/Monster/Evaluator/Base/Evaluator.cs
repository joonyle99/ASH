using System.Collections;
using UnityEngine;

/// <summary>
/// �ǵ��� Ŭ����
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
        // Debug.Log("CoolTime ����");

        IsDuringCoolTime = true;
        yield return new WaitForSeconds(_targetCheckCoolTime);
        IsDuringCoolTime = false;

        // Debug.Log("CoolTime ��");
    }
    public virtual void StartCoolTimeCoroutine()
    {
        if (_targetCheckCoolTime < 0.01f)
        {
            Debug.LogWarning("CheckCoolTime�� ����ҰŸ� ���� �������ּ��� ");
            return;
        }

        if (this._coolTimeCoroutine != null)
            StopCoroutine(_coolTimeCoroutine);

        this._coolTimeCoroutine = StartCoroutine(CoolTimeCoroutine());
    }
}
