using System.Collections;
using UnityEngine;

public class Bear_GroundWave : Monster_IndependentSkill
{
    [Header("Ground Wave")]
    [Space]

    [SerializeField] private Vector2 _moveDir = Vector2.zero;
    [SerializeField] private float _speed;
    [SerializeField] private float _targetDistance;
    [SerializeField] private float _elapsedMoveDistance;

    [Space]

    [SerializeField] private bool isStopped;

    private void Update()
    {
        if (isStopped)
            return;

        // 프레임당 이동 벡터 및 거리 계산
        var frameMoveVector = _speed * _moveDir * Time.deltaTime;
        var frameMoveDistance = frameMoveVector.magnitude;
        transform.Translate(frameMoveVector);

        // 프레임당 이동 거리 누적
        _elapsedMoveDistance += frameMoveDistance;

        // 누적 이동 거리가 목표 거리를 넘어서면 삭제
        if (_elapsedMoveDistance >= _targetDistance)
            StartCoroutine(DestroyProcess());
    }

    public void SetDir(Vector2 dir)
    {
        // 이동 방향 설정
        _moveDir = dir;

        if (dir.x < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    public IEnumerator DestroyProcess()
    {
        // TODO: 일정 거리 도달 시 Collider2D 컴포넌트를 비활성화 하는데, Trail에 따른 정교한 충돌 처리가 필요하다면 파티클 시스템의 Collision을 사용한다.

        isStopped = true;

        var col = GetComponent<Collider2D>();       // 일정 거리에 도달하면 콜라이더 비활성화
        col.enabled = false;

        yield return new WaitForSeconds(2f);        // Trail을 마저 출력한 후 파괴한다

        DestroyImmediately();
    }
}
