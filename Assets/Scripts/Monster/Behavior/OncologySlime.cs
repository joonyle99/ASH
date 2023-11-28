using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���罽���� ���� Ŭ����
/// </summary>
public class OncologySlime : NormalMonster
{
    #region Attribute

    private SpriteRenderer _renderer;                             // ���� ����
    [SerializeField] private List<Transform> _wayPoints;          // ������ ���
    private Transform _currTransform;                             // ������
    private Transform _nextTransform;                             // ���� ������
    private int _currentWaypointIndex;                            // ������ �ε���
    private float _moveSpeed;                                     // ���� �̵� �ӵ�
    private float _upPower;                                       // ƨ��� ��
    private GameObject _player;                                   // �÷��̾� ����
    [Range(0f, 50f)] private float _volumeMul;                    // ���� ���
    [Range(0f, 1000f)] private float _power;                      // �˹� �Ŀ�
    [Range(0, 100)] private int _damage;                          // ������

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

        // �÷��̾� ���ӿ�����Ʈ
        // _player = SceneContextController.Player.gameObject; -> ����
        _player = GameObject.Find("Player");

        // �ʱ� ������
        _currTransform = _wayPoints[_currentWaypointIndex];
        _nextTransform = _wayPoints[(_currentWaypointIndex + 1) % _wayPoints.Count];

        _renderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void Update()
    {
        base.Update();

        // �������� ���� �������� �̵�
        if (Vector3.Distance(_currTransform.position,
                transform.position) < 2f)
        {
            _currentWaypointIndex++;
            _currentWaypointIndex %= _wayPoints.Count;
            _currTransform = _wayPoints[_currentWaypointIndex];
            _nextTransform = _wayPoints[(_currentWaypointIndex + 1) % _wayPoints.Count];
        }
    }

    public override void SetUp()
    {
        // �⺻ �ʱ�ȭ
        base.SetUp();

        // ���� �������� �ִ� ü��
        MaxHp = 100;

        // ���� �������� ���� ü��
        CurHp = MaxHp;

        // ���� �������� ID ����
        ID = 1001;

        // ���� �������� �̸� ����
        MonsterName = "���� ������";

        // ũ��
        Size = SIZE.Small;

        // ���� �������� Ȱ�� ����
        ActionType = ACTION_TYPE.Floating;

        // ����
        Response = RESPONE.None;

        // ����
        IsAggressive = IS_AGGRESSIVE.Peace;

        // ����
        IsChase = IS_CHASE.AllTerritory;

        // ����
        IsRunaway = IS_RUNAWAY.Aggressive;
    }

    public override void OnDamage(int _damage)
    {
        //Debug.Log("slime _damage");
        base.OnDamage(_damage);
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
        float startAlpha = _renderer.color.a;

        // ������ ���İ� ����
        float t = 0;
        while (t < 3)
        {
            t += Time.deltaTime;
            float normalizedTime = t / 2;
            Color color = _renderer.color;
            color.a = Mathf.Lerp(startAlpha, 0f, normalizedTime);
            _renderer.color = color;
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
            // �Ÿ��� ����ؼ� ���� �Ҹ��� Ű���
            float finalMul = 1 / Vector3.Distance(_player.transform.position, transform.position) * _volumeMul;
            if (finalMul > 1f)
                finalMul = 1f;
            GetComponent<SoundList>().PlaySFX("SE_Slime", finalMul);

            // ���� ����� �� ���� �ຼ��?
            if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Vector3 moveDirection = (_currTransform.position - transform.position).normalized;
                Vector3 force = new Vector3(moveDirection.x * _moveSpeed, _upPower, 0f);
                Rigidbody.velocity = force;
                //Rigidbody.velocity = Vector2.zero;
                //Rigidbody.AddForce(force);
            }
        }
        // �÷��̾�� �浹���� ��
        else if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (collision.collider.gameObject.GetComponent<PlayerBehaviour>().CurHp == 0)
                return;

            Debug.Log("�÷��̾�� �浹");

            float dir = Mathf.Sign(collision.transform.position.x - transform.position.x);
            Vector2 vec = new Vector2(_power * dir, _power);
            // collision.gameObject.GetComponent<PlayerBehaviour>().OnHit(_damage, vec);
        }
    }

    #endregion
}