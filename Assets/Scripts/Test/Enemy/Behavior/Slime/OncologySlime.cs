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

    public override void SetUp(string name, int maxHp, TYPE type, ACTION_TYPE aType)
    {
        // �⺻ �ʱ�ȭ
        base.SetUp(name, maxHp, type, aType);

        // �߰� �ʱ�ȭ �ɼ�

        // ID ����
        ID = 1001;

        // Ȱ�� ����
        ActionType = aType;
    }

    public override void OnDamage(int _damage)
    {
        // �⺻ �ǰ�
        base.OnDamage(_damage);

        // �߰� �ǰ� �ɼ�

    }

    public override void KnockBack(Vector2 vec)
    {
        base.KnockBack(vec);

        //Rigidbody.AddForce(vec);
    }

    public override void Die()
    {
        // �⺻ ���
        base.Die();

        // �߰� ��� �ɼ�

    }
}