using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NormalMonster : BasedMonster
    // �Ϲ� ���� Ŭ���� (�߻� Ŭ����)
{
    // Noraml Monster members
    // ...

    public override void SetUp(string name, int maxHp)
    {
        // �⺻ �ʱ�ȭ
        base.SetUp(name, maxHp);
        //Debug.Log("normal");

        // �߰� �ʱ�ȭ �ɼ�

    }

    public override void OnDamage(int _damage)
    {
        // �⺻ �ǰ�
        base.OnDamage(_damage);

        // �߰� �ǰ� �ɼ�

    }

    public override void Die()
    {
        // �⺻ ���
        base.Die();

        // �߰� ��� �ɼ�

    }
}
