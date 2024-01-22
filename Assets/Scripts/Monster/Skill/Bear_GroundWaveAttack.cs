using UnityEngine;

public class Bear_GroundWaveAttack : Monster_SkillAttack
{
    [Header("Bear_GroundWaveAttack")]
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
}
