using UnityEngine;

public class IdleState : PlayerState
{
    /// <summary>
    /// 기울어진 땅에서 미끄러지지 않도록 하는 변수들
    /// </summary>
    Vector2 _groundNormal;          // 땅의 법선벡터
    Vector3 _groundHitPoint;        // 땅의 Hit Point
    float _forcePower = 100f;       // 아래로 가해주는 힘
    public float _angle = 0f;

    protected override void OnEnter()
    {

    }

    protected override void OnUpdate()
    {
        // 땅의 법선벡터
        _groundNormal = Player.GroundHit.normal;

        // 땅의 Hit Point
        _groundHitPoint = Player.GroundHit.point;

        // Run State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) != 0)
        {
            ChangeState<RunState>();
            return;
        }

        _angle = Vector3.Angle(_groundNormal, Player.PlayerLookDir);
        if (Mathf.Abs(90 - _angle) > 10f)
            Player.Rigidbody.velocity = new Vector2(-_groundNormal.x, -_groundNormal.y) * _forcePower * Time.deltaTime;
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
        Gizmos.DrawLine(_groundHitPoint, _groundHitPoint + new Vector3(_groundNormal.x, _groundNormal.y, 0f));
    }
}