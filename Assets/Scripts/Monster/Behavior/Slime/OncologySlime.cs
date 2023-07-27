using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// ���罽���� ���� Ŭ����
/// </summary>
public class OncologySlime : NormalMonster
{
    public SpriteRenderer renderer;                             // ���� ����
    [SerializeField] public List<Transform> wayPoints;          // ������ ���
    public Transform currTransform;                             // ������
    public Transform nextTransform;                             // ���� ������
    public int currentWaypointIndex;                            // ������ �ε���
    public float moveSpeed;                                     // ���� �̵� �ӵ�
    public float upPower;                                       // ƨ��� ��
    public GameObject player;                                   // �÷��̾� ����
    [Range(0f, 50f)] public float volumeMul;                    // ���� ���
    [Range(0f, 1000f)] public float power;                      // �˹� �Ŀ�
    [Range(0, 100)] public int damage;                          // ������

    protected override void Start()
    {
        base.Start();

        // �ʱ� ����
        SetUp();

        // �÷��̾� ���ӿ�����Ʈ
        // player = SceneContextController.Player.gameObject; -> ����
        player = GameObject.Find("Player");

        // �ʱ� ������
        currTransform = wayPoints[currentWaypointIndex];
        nextTransform = wayPoints[currentWaypointIndex + 1];

        renderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void Update()
    {
        base.Update();

        // �������� ���� �������� �̵�
        if (Vector3.Distance(currTransform.position,
                transform.position) < 2f)
        {
            currentWaypointIndex++;
            currentWaypointIndex %= wayPoints.Count;
            currTransform = wayPoints[currentWaypointIndex];
            nextTransform = wayPoints[(currentWaypointIndex + 1) % wayPoints.Count];
        }
    }

    public override void SetUp()
    {
        // �⺻ �ʱ�ȭ
        base.SetUp();

        // ���� �������� �ִ� ü��
        MaxHp = 100;

        // ���� �������� ���� ü��
        CurHP = MaxHp;

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
        //Debug.Log("slime damage");
        base.OnDamage(_damage);
    }

    public override void KnockBack(Vector2 vec)
    {
        this.Rigidbody.AddForce(vec);
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
        float startAlpha = renderer.color.a;

        // ������ ���İ� ����
        float t = 0;
        while (t < 3)
        {
            t += Time.deltaTime;
            float normalizedTime = t / 2;
            Color color = renderer.color;
            color.a = Mathf.Lerp(startAlpha, 0f, normalizedTime);
            renderer.color = color;
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
            float finalMul = 1 / Vector3.Distance(player.transform.position, transform.position) * volumeMul;
            if (finalMul > 1f)
                finalMul = 1f;
            GetComponent<SoundList>().PlaySFX("SE_Slime", finalMul);

            // ���� ����� �� ���� �ຼ��?
            if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Vector3 moveDirection = (currTransform.position - transform.position).normalized;
                Vector3 force = new Vector3(moveDirection.x * moveSpeed, upPower, 0f);
                Rigidbody.velocity = Vector2.zero;
                Rigidbody.AddForce(force);
            }
        }
        // �÷��̾�� �浹���� ��
        else if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("�÷��̾�� �浹");

            float dir = Mathf.Sign(collision.transform.position.x - transform.position.x);
            Vector2 vec = new Vector2(power * dir, power);
            collision.gameObject.GetComponent<PlayerBehaviour>().OnHit(damage, vec);
        }
    }
}