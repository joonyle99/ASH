using UnityEngine;

public class Bear_GroundWave : Monster_SkillObject
{
    [Header("Bear_GroundWave")]
    [Space]

    [SerializeField] private Vector2 _moveDir = Vector2.zero;

    [SerializeField] private float _targetDistance = 14f;   // 14����
    [SerializeField] private float _elapsedMoveDistance;

    private void Update()
    {
        // �����Ӵ� �̵� ���� �� �Ÿ� ���
        Vector2 frameMoveVector = _moveDir * Time.deltaTime * 14f;
        float frameMoveDistance = frameMoveVector.magnitude;
        transform.Translate(frameMoveVector);

        // �����Ӵ� �̵� �Ÿ� ����
        _elapsedMoveDistance += frameMoveDistance;
        // ���� �̵� �Ÿ��� ��ǥ �Ÿ��� �Ѿ�� ����
        if (_elapsedMoveDistance >= _targetDistance)
            Destroy(this.gameObject);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    public void SetDir(Vector2 dir)
    {
        // �̵� ���� ����
        _moveDir = dir;
    }

    /*
    // TODO : ������ ���� ���󰡵��� ����
    public void GroundWalking()
    {
        Vector2 groundNormal = GroundRayHit.normal;
        Vector2 moveDirection = RecentDir > 0
            ? (-1) * Vector2.Perpendicular(groundNormal)
            : Vector2.Perpendicular(groundNormal);

        Debug.DrawRay(GroundRayHit.point, groundNormal);

        Vector2 targetVelocity = moveDirection * MoveSpeed;
        Vector2 velocityNeeded = targetVelocity - Vector2.Dot(Rigidbody.velocity, moveDirection) * moveDirection;
        Vector2 moveForce = velocityNeeded * Acceleration;

        Debug.DrawRay(transform.position, moveForce);

        Rigidbody.AddForce(moveForce);
    }
    */
}
