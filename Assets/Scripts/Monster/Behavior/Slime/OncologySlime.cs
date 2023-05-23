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

        // ���� �������� Ȱ�� ����
        ActionType = ACTION_TYPE.Ground;
    }

    public override void OnDamage(int _damage)
    {
        base.OnDamage(_damage);
    }

    public override void KnockBack(Vector2 vec)
    {
        base.KnockBack(vec);

        this.Rigidbody.AddForce(vec);
        Debug.Log("���罽���� AddForce");
    }

    public override void Die()
    {
        base.Die();

    }
}