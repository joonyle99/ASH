using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gizmos = UnityEngine.Gizmos;

/// <summary>
/// ������ ���� Ŭ����
/// </summary>
public class Slime : NormalMonster
{
    #region Attribute

    [Header("Slime")]
    [Space]

    [SerializeField] private Transform _wayPointBox;
    [SerializeField] private List<Transform> _wayPoints;
    [SerializeField] private Transform _curTargetPosition;
    [SerializeField] private Transform _nextTargetPosition;
    [SerializeField] private int _curWayPointIndex = 0;
    [SerializeField] private float _distanceWithTarget = 2f;
    [SerializeField] private Vector3 _moveDir;
    [SerializeField] private float _upperPower = 10f;
    [SerializeField] private float _volumeMul;
    [SerializeField] private float _power;

    #endregion

    #region Function

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < _wayPointBox.childCount; ++i)
            _wayPoints.Add(_wayPointBox.GetChild(i));

        // �ʱ� ������
        _curTargetPosition = _wayPoints[_curWayPointIndex];
        _nextTargetPosition = _wayPoints[(_curWayPointIndex + 1) % _wayPoints.Count];
    }

    protected override void Update()
    {
        base.Update();

        // �������� ���� �������� �̵�
        if (Vector3.Distance(_curTargetPosition.position,
                transform.position) < _distanceWithTarget)
        {
            _curWayPointIndex++;
            _curTargetPosition = _nextTargetPosition;
            _nextTargetPosition = _wayPoints[(_curWayPointIndex + 1) % _wayPoints.Count];
        }
    }

    public override void SetUp()
    {
        // �⺻ �ʱ�ȭ
        base.SetUp();
    }

    public override void OnDamage(int damage)
    {
        base.OnDamage(damage);
    }

    public override void KnockBack(Vector2 force)
    {
        base.KnockBack(force);
    }

    public override void Die()
    {
        base.Die();
    }

    /// <summary>
    /// ���̳� ���� Collision �浹
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ���̳� ���� �浹���� ��
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Wall") ||
            collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // �Ÿ��� �ݺ���ؼ� ���� �Ҹ��� Ű���
            float finalMul = 1 / Vector3.Distance(SceneContext.Current.Player.transform.position, this.transform.position) * _volumeMul;
            if (finalMul > 1f)
                finalMul = 1f;
            GetComponent<SoundList>().PlaySFX("SE_Slime", finalMul);

            // ���� ����� �� ���� �ຼ��?
            if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                // TODO : Ƣ������� �ý��� ����
                float dir = Mathf.Sign(_curTargetPosition.position.x - transform.position.x);
                Vector2 moveVector = new Vector2(dir * MoveSpeed, _upperPower);
                _moveDir = moveVector.normalized;
                Rigidbody.AddForce(moveVector, ForceMode2D.Impulse);
            }
        }
        // �÷��̾�� �浹���� ��
        else if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            /*
            if (collision.collider.gameObject.GetComponent<PlayerBehaviour>().CurHp == 0)
                return;

            Debug.Log("�÷��̾�� �浹");

            float dir = Mathf.Sign(collision.transform.position.x - transform.position.x);
            Vector2 vec = new Vector2(_power * dir, _power);

            // collision.gameObject.GetComponent<PlayerBehaviour>().OnHit(damage, vec);
            */
        }
    }
    private void OnDrawGizmosSelected()
    {
        // �̵� ����
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(this.transform.position, this.transform.position + _moveDir * 2f);
    }

    #endregion
}