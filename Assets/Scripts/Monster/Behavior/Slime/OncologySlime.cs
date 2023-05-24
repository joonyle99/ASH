using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OncologySlime : NormalMonster
    // ���罽���� ���� Ŭ����
{
    // slime member
    // ...

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
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
        // Response = RESPONE.None;

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

        //this.Rigidbody.AddForce(vec);
        this.Rigidbody.velocity = vec;
        this.Rigidbody.gravityScale = 1f;
    }

    public override void Die()
    {
        base.Die();

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ���̾ Wall�̸�
        if(collision.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            this.Rigidbody.velocity = Vector2.zero;
            this.Rigidbody.gravityScale = 0f;
        }

    }

    //public override void OnTriggerEnter2D(Collider2D collision)
    //{
    //    // �ǰ� collision���� "PlayerBasicAttackHitbox" ������Ʈ�� ã��
    //    if (collision.GetComponent<PlayerBasicAttackHitbox>() != null)
    //    {
    //        Debug.Log("Hitted by basic attack");
    //    }
    //}
}