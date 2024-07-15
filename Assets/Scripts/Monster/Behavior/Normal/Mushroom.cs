using UnityEngine;

public sealed class Mushroom : MonsterBehaviour, ILightCaptureListener
{
    [Header("Mushroom")]
    [Space]

    [SerializeField] private float _targetDieTime = 4f;
    [SerializeField] private float _elapsedDieTime;

    [Space]

    [SerializeField] private LayerMask _filterLayer;

    // awake 오버라이드
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
            // Debug.Log("Hide State로 전이한다");

            // 빛에 닿으면 숨는다
            SetAnimatorTrigger("Hide");
        }

        _elapsedDieTime += Time.deltaTime;

        // 일정 시간이 지나면 죽는다
        if (_elapsedDieTime > _targetDieTime)
            CurHp -= monsterData.MaxHp;

        // 자동 전환을 막는다
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
        // 몬스터에서 플레이어 방향으로의 벡터를 계산합니다.
        Vector3 directionToPlayer = targetPoint - transform.position;

        // Ray를 발사하고 결과를 확인합니다.
        if (Physics2D.Raycast(transform.position, directionToPlayer.normalized, directionToPlayer.magnitude, _filterLayer.value))
        {
            // Ray가 TargetLayer에 해당하는 오브젝트에 부딪혔다면 true를 반환합니다.
            Debug.DrawRay(transform.position, directionToPlayer, Color.magenta, 2f);
            return true;
        }

        // 장애물이 없다면 false를 반환합니다.
        return false;
    }
}
