using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class OncologySlime : NormalMonster
    // ���罽���� ���� Ŭ����
{
    // slime member
    // ...

    public SpriteRenderer renderer;         // ���� ����

    [SerializeField]
    public List<Transform> wayPoints;       // ������ ���
    public Transform currTransform;         // ������
    public Transform nextTransform;         // ���� ������
    public int currentWaypointIndex;        // ������ �ε���
    public float moveSpeed;                 // ���� �̵� �ӵ�
    public float time;

    protected override void Start()
    {
        base.Start();

        // �ʱ� ����
        SetUp();

        // �ʱ� ������
        currTransform = wayPoints[currentWaypointIndex];
        nextTransform = wayPoints[currentWaypointIndex + 1];
    }

    protected override void Update()
    {
        base.Update();

        Debug.Log(Vector3.Distance(currTransform.position,
            transform.position));

        // ���͸� ���� �������� �̵���ŵ�ϴ�.
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
        base.OnDamage(_damage);
    }

    public override void KnockBack(Vector2 vec)
    {
        base.KnockBack(vec);

        this.Rigidbody.AddForce(vec);
    }

    public override void Die()
    {
        base.Die();

        // �浹 ��Ȱ��ȭ
        gameObject.GetComponent<CircleCollider2D>().enabled = false;

        // ������� ����
        StartCoroutine(FadeOutObject());
    }

    private IEnumerator FadeOutObject()
    {
        // �ʱ� ���İ� ����
        float startAlpha = renderer.material.color.a;

        // ������ ���İ� ����
        float t = 0;
        while (t < 2)
        {
            t += Time.deltaTime;
            float normalizedTime = t / 2;
            Color color = renderer.material.color;
            color.a = Mathf.Lerp(startAlpha, 0f, normalizedTime);
            renderer.material.color = color;
            yield return null;
        }

        // ������Ʈ ��Ȱ��ȭ
        Destroy(gameObject);
    }

    /// <summary>
    /// ���̳� ���� Collision �浹
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            GetComponent<SoundList>().PlaySFX("SE_Slime");

        // ���� ����� �� ���� �ຼ��?
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("Push");

            Vector3 moveDirection = (currTransform.position - transform.position).normalized;
            Vector3 force = new Vector3(moveDirection.x * moveSpeed, 200f, 0f);
            Rigidbody.AddForce(force);
        }
    }
}