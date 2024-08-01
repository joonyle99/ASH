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

        // �����Ӵ� �̵� ���� �� �Ÿ� ���
        var frameMoveVector = _speed * _moveDir * Time.deltaTime;
        var frameMoveDistance = frameMoveVector.magnitude;
        transform.Translate(frameMoveVector);

        // �����Ӵ� �̵� �Ÿ� ����
        _elapsedMoveDistance += frameMoveDistance;

        // ���� �̵� �Ÿ��� ��ǥ �Ÿ��� �Ѿ�� ����
        if (_elapsedMoveDistance >= _targetDistance)
            StartCoroutine(DestroyProcess());
    }

    public void SetDir(Vector2 dir)
    {
        // �̵� ���� ����
        _moveDir = dir;

        if (dir.x < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    public IEnumerator DestroyProcess()
    {
        // TODO: ���� �Ÿ� ���� �� Collider2D ������Ʈ�� ��Ȱ��ȭ �ϴµ�, Trail�� ���� ������ �浹 ó���� �ʿ��ϴٸ� ��ƼŬ �ý����� Collision�� ����Ѵ�.

        isStopped = true;

        var col = GetComponent<Collider2D>();       // ���� �Ÿ��� �����ϸ� �ݶ��̴� ��Ȱ��ȭ
        col.enabled = false;

        yield return new WaitForSeconds(2f);        // Trail�� ���� ����� �� �ı��Ѵ�

        DestroyImmediately();
    }
}
