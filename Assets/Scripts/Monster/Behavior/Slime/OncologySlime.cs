using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���罽���� ���� Ŭ����
/// </summary>
public class OncologySlime : NormalMonster
{
    public SpriteRenderer renderer;         // ���� ����

    [SerializeField]
    public List<Transform> wayPoints;       // ������ ���
    public Transform currTransform;         // ������
    public Transform nextTransform;         // ���� ������
    public int currentWaypointIndex;        // ������ �ε���
    public float moveSpeed;                 // ���� �̵� �ӵ�
    public float upPower;                   // ƨ��� ��
    public GameObject player;
    [Range(0f, 50f)] public float volumeMul;

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

        // �������� ���� �������� �̵���ŵ�ϴ�.
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
        Debug.Log("slime damage");
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

        // ������Ʈ ����
        Destroy(gameObject);
    }

    /// <summary>
    /// ���̳� ���� Collision �浹
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Wall") ||
            collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // �Ÿ��� ����ؼ� ���� �Ҹ��� Ű���
            float finalMul = 1 / Vector3.Distance(player.transform.position, transform.position) * volumeMul;
            if (finalMul > 1f)
                finalMul = 1f;
            Debug.Log(finalMul);
            GetComponent<SoundList>().PlaySFX("SE_Slime", finalMul);

            // ���� ����� �� ���� �ຼ��?
            if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Debug.Log("Push");

                Vector3 moveDirection = (currTransform.position - transform.position).normalized;
                Vector3 force = new Vector3(moveDirection.x * moveSpeed, upPower, 0f);
                Rigidbody.AddForce(force);
            }
        }
        else if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("�÷��̾�� ����");
        }
    }
}