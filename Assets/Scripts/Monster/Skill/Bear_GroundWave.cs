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

        // �����Ӵ� �̵� ���� �� �Ÿ� ���
        Vector2 frameMoveVector = _moveDir * Time.deltaTime * _speed;
        float frameMoveDistance = frameMoveVector.magnitude;
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
        isStopped = true;
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }
}
