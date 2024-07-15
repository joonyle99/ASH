using UnityEngine;

public sealed class Mushroom : MonsterBehaviour, ILightCaptureListener
{
    [Header("Mushroom")]
    [Space]

    [SerializeField] private float _targetDieTime = 4f;
    [SerializeField] private float _elapsedDieTime;

    [Space]

    [SerializeField] private LayerMask _filterLayer;

    // awake �������̵�
    protected override void Awake()
    {
        base.Awake();

        AttackEvaluator.FilterEvent -= HasObstacleBetween;
        AttackEvaluator.FilterEvent += HasObstacleBetween;
    }

    public void OnLightStay(LightCapturer capturer, LightSource lightSource)
    {
        if (IsDead) return;

        if (CurrentStateIs<Monster_IdleState>())
        {
            // Debug.Log("Hide State�� �����Ѵ�");

            // ���� ������ ���´�
            SetAnimatorTrigger("Hide");
        }

        _elapsedDieTime += Time.deltaTime;

        // ���� �ð��� ������ �״´�
        if (_elapsedDieTime > _targetDieTime)
            CurHp -= monsterData.MaxHp;

        // �ڵ� ��ȯ�� ���´�
        if (CurrentStateIs<Monster_HideState>())
        {
            if (CurrentState.IsAutoStateTransition)
                CurrentState.ElapsedStayTime = 0f;
        }
    }

    private void OnDestroy()
    {
        AttackEvaluator.FilterEvent -= HasObstacleBetween;
    }

    public void OnLightExit(LightCapturer capturer, LightSource lightSource)
    {
        _elapsedDieTime = 0f;
    }

    public bool HasObstacleBetween(Vector3 targetPoint)
    {
        // ���Ϳ��� �÷��̾� ���������� ���͸� ����մϴ�.
        Vector3 directionToPlayer = targetPoint - transform.position;

        // Ray�� �߻��ϰ� ����� Ȯ���մϴ�.
        if (Physics2D.Raycast(transform.position, directionToPlayer.normalized, directionToPlayer.magnitude, _filterLayer.value))
        {
            // Ray�� TargetLayer�� �ش��ϴ� ������Ʈ�� �ε����ٸ� true�� ��ȯ�մϴ�.
            Debug.DrawRay(transform.position, directionToPlayer, Color.magenta, 2f);
            return true;
        }

        // ��ֹ��� ���ٸ� false�� ��ȯ�մϴ�.
        return false;
    }
}
