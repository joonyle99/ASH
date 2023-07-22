using UnityEngine;

public class IdleState : PlayerState
{
    [Header("Idle Setting")]

    [Space]

    float _belowForce = 100f;       // ¾Æ·¡·Î °¡ÇØÁÖ´Â Èû

    Vector2 _groundNormal;          // ¶¥ÀÇ ¹ý¼±º¤ÅÍ
    Vector3 _groundHitPoint;        // ¶¥ÀÇ Hit Point
    float _angle;

    protected override void OnEnter()
    {

    }

    protected override void OnUpdate()
    {
        // ¶¥ÀÇ ¹ý¼±º¤ÅÍ
        _groundNormal = Player.GroundHit.normal;

        // ¶¥ÀÇ Hit Point
        _groundHitPoint = Player.GroundHit.point;

        // Run State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) != 0)
        {
            ChangeState<RunState>();
            return;
        }

        _angle = Vector3.Angle(_groundNormal, Player.PlayerLookDir);
        if (Mathf.Abs(90f - _angle) > 5f)
            Player.Rigidbody.velocity = new Vector2(-_groundNormal.x, -_groundNormal.y) * _belowForce * Time.deltaTime;
    }

    protected override void OnExit()
    {

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_groundHitPoint, _groundHitPoint + new Vector3(_groundNormal.x, _groundNormal.y, 0f));
    }
}