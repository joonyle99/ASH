using UnityEngine;

public class IdleState : PlayerState
{
    [Header("Idle Setting")]
    Vector2 _groundNormal;
    Vector3 _tempGroundHitPoint;

    protected override void OnEnter()
    {
        //Debug.Log("Idle Enter");

        // Exception
        if (!Player.GroundHit)
            return;

        // ¶¥ÀÇ ¹ý¼±º¤ÅÍ
        _groundNormal = Player.GroundHit.normal;

        // ¶¥ÀÇ raycast hit À§Ä¡°ª
        _tempGroundHitPoint = Player.GroundHit.point;
    }

    protected override void OnUpdate()
    {
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) != 0)
        {
            ChangeState<WalkState>();
            return;
        }

        // º®¿¡¼­ ¹Ì²ô·¯ÁöÁö ¾ÊÀ½
        float angle = Vector3.Angle(_groundNormal, Player.PlayerLookDir);
        if (Mathf.Abs(90 - angle) > 10f)
            Player.Rigidbody.velocity = new Vector2((-1) * _groundNormal.x, (-1) * _groundNormal.y) * 100f * Time.deltaTime;

    }

    protected override void OnExit()
    {
        //Debug.Log("Idle Exit");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_tempGroundHitPoint, _tempGroundHitPoint + new Vector3(_groundNormal.x, _groundNormal.y, 0f));
    }
}