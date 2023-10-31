using UnityEngine;

public class IdleState : PlayerState
{
    [Header("Idle Setting")]

    [Space]

    [SerializeField] float _belowForce = 6000f;       // ¾Æ·¡·Î °¡ÇØÁÖ´Â Èû

    Vector2 _groundNormal;                            // ¶¥ÀÇ ¹ý¼±º¤ÅÍ
    Vector3 _groundHitPoint;                          // ¶¥ÀÇ Hit Point

    protected override void OnEnter()
    {
        // ¶¥ÀÇ ¹ý¼±º¤ÅÍ
        _groundNormal = Player.GroundHit.normal;

        // ¶¥ÀÇ Hit Point
        _groundHitPoint = Player.GroundHit.point;
    }

    protected override void OnUpdate()
    {
        // Run State
        if (Mathf.RoundToInt(Player.RawInputs.Movement.x) != 0)
        {
            ChangeState<RunState>();
            return;
        }

        // ÇÃ·¹ÀÌ¾î¿Í ¶¥ »çÀÌÀÇ °¢µµ
        float _angle = Vector2.Angle(_groundNormal, Player.PlayerLookDir);

        // ±â¿ï¾îÁø ¶¥¿¡¼­ ¹Ì²ô·³ ¹æÁö
        if (Mathf.Abs(90f - _angle) > 10f)
        {
            // Debug.Log("±â¿ï¾îÁø ¶¥ÀÔ´Ï´Ù");
            Player.Rigidbody.AddForce(-_groundNormal * _belowForce * Time.deltaTime);
        }
        else
        {
            // Debug.Log("ÆòÆòÇÑ ¶¥ÀÔ´Ï´Ù");
            Player.Rigidbody.AddForce(-_groundNormal * _belowForce / 3f * Time.deltaTime);
        }


    }
    protected override void OnFixedUpdate()
    {

    }

    protected override void OnExit()
    {

    }

    // private void OnDrawGizmosSelected()
    private void OnDrawGizmos()
    {
        // ¶¥ÀÇ ¹ý¼±º¤ÅÍ ±×¸®±â
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_groundHitPoint, _groundHitPoint + new Vector3(_groundNormal.x, _groundNormal.y, 0f));
    }
}