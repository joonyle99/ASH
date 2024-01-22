using UnityEngine;

public class Bear_GroundWaveAttack : Monster_SkillAttack
{
    [Header("Bear_GroundWaveAttack")]
    [Space]

    [SerializeField] private Vector2 _moveDir = Vector2.zero;

    [SerializeField] private float _targetDistance = 14f;   // 14유닛
    [SerializeField] private float _elapsedMoveDistance;

    private void Update()
    {
        // 프레임당 이동 벡터 및 거리 계산
        Vector2 frameMoveVector = _moveDir * Time.deltaTime * 14f;
        float frameMoveDistance = frameMoveVector.magnitude;
        transform.Translate(frameMoveVector);

        // 프레임당 이동 거리 누적
        _elapsedMoveDistance += frameMoveDistance;
        // 누적 이동 거리가 목표 거리를 넘어서면 삭제
        if (_elapsedMoveDistance >= _targetDistance)
            Destroy(this.gameObject);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    public void SetDir(Vector2 dir)
    {
        // 이동 방향 설정
        _moveDir = dir;
    }
}
