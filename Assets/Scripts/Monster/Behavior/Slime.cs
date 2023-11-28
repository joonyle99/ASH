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

    [SerializeField] private List<Transform> _wayPoints;
    [SerializeField] private Transform _curTargetPosition;
    [SerializeField] private Transform _nextTargetPosition;
    [SerializeField] private int _curWayPointIndex = 0;
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

        // �ʱ� ����
        SetUp();

        // �ʱ� ������
        _curTargetPosition = _wayPoints[_curWayPointIndex];
        _nextTargetPosition = _wayPoints[(_curWayPointIndex + 1) % _wayPoints.Count];
    }

    protected override void Update()
    {
        base.Update();

        // �������� ���� �������� �̵�
        if (Vector3.Distance(_curTargetPosition.position,
                transform.position) < 2f)
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

        // �������� ID ����
        ID = 1001;

        // �������� �̸� ����
        MonsterName = "���� ������";

        // �������� �ִ� ü��
        MaxHp = 200;

        // �������� ���� ü��
        CurHp = MaxHp;

        // ������ �̵��ӵ�
        MoveSpeed = 3;

        // ũ��
        MonsterSize = MONSTER_SIZE.Small;

        // �������� Ȱ�� ����
        ActionType = ACTION_TYPE.Floating;

        // ����
        ResponseType = RESPONE_TYPE.None;

        // ����
        AggressiveType = AGGRESSIVE_TYPE.Peace;

        // ����
        ChaseType = CHASE_TYPE.AllTerritory;

        // ����
        RunawayType = RUNAWAY_TYPE.Aggressive;
    }

    public override void OnDamage(int damage)
    {
        base.OnDamage(damage);
    }

    public override void KnockBack(Vector2 vec)
    {
        Rigidbody.velocity = vec;
    }

    public override void Die()
    {
        base.Die();

        // ������� ����
        StartCoroutine(FadeOutObject());
    }

    private IEnumerator FadeOutObject()
    {
        // �ʱ� ���İ� ����
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float startAlpha = spriteRenderer.color.a;

        // ������ ���İ� ����
        float t = 0;
        while (t < 3)
        {
            t += Time.deltaTime;
            float normalizedTime = t / 2;
            Color color = spriteRenderer.color;
            color.a = Mathf.Lerp(startAlpha, 0f, normalizedTime);
            spriteRenderer.color = color;
            yield return null;
        }

        // ������Ʈ ����
        Destroy(gameObject);
        yield return null;
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
                Vector3 moveDirection = (_curTargetPosition.position - transform.position).normalized;
                Vector3 force = new Vector3(moveDirection.x * MoveSpeed, _upperPower, 0f);
                Rigidbody.velocity = force;
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

    #endregion
}