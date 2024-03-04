using System.Collections;
using UnityEngine;

public class Bear_GroundWave : Monster_SkillObject
{
    [Header("Bear_GroundWave")]
    [Space]

    [SerializeField] private Vector2 _moveDir = Vector2.zero;

    [SerializeField] private float _targetDistance;
    [SerializeField] private float _elapsedMoveDistance;
    [SerializeField] private float _speed = 8f;

    [SerializeField] private bool isStopped = false;

    private void Update()
    {
        if (isStopped)
            return;

        // 프레임당 이동 벡터 및 거리 계산
        Vector2 frameMoveVector = _moveDir * Time.deltaTime * _speed;
        float frameMoveDistance = frameMoveVector.magnitude;
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
        isStopped = true;
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }
}
