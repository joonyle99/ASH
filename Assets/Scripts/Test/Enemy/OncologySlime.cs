using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OncologySlime : NormalMonster
    // ���罽���� ���� Ŭ����
{
    // slime member
    // ...

    public override void SetUp(string name, int maxHp)
    {
        // �⺻ �ʱ�ȭ
        base.SetUp(name, maxHp);
        //Debug.Log("slime");

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

    private void Start()
    {
        Debug.Log(MonsterName + "�� �����Ǿ����ϴ�");
    }

    private void Update()
    {
        //Debug.Log(this.gameObject.name + "�� Update�� �����ִ�");
    }
}
