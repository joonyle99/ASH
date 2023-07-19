using UnityEngine;

public class IdleState : PlayerState
{
    Vector2 _groundNormal;
    Vector3 _tempGroundHitPoint;
    float _forcePower = 100f;

    protected override void OnEnter()
    {
        // 플레이어가 땅을 밟고있지 않은 경우
        if (!Player.GroundHit)
            return;

        // 땅의 법선벡터
        _groundNormal = Player.GroundHit.normal;

        // 땅의 raycast hit 위치값
        _tempGroundHitPoint = Player.GroundHit.point;
    }

    protected override void OnUpdate()
    {
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) != 0)
        {
            ChangeState<RunState>();
            return;
        }

        // 벽에서 미끄러지지 않음 (땅에서는 처리 X)
        float angle = Vector3.Angle(_groundNormal, Player.PlayerLookDir);
        if (Mathf.Abs(90 - angle) > 5f)
            Player.Rigidbody.velocity = new Vector2((-1) * _groundNormal.x, (-1) * _groundNormal.y) * _forcePower * Time.deltaTime;
    }

    protected override void OnExit()
    {

    }

    /// <summary>
    /// 땅의 법선벡터 시각화
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_tempGroundHitPoint, _tempGroundHitPoint + new Vector3(_groundNormal.x, _groundNormal.y, 0f));
    }
}