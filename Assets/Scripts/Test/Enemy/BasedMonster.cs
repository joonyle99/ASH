using UnityEngine;

/*
public enum SIZE
{
    Small = 0,
    Medium,
    Large
}

public enum TYPE
{
    Normal = 0,
    SemiBoss,
    Boss
}

public enum ACTION_TYPE
{
    Ground = 0,
    Floating,
    Crawl
}

public enum RESPONE
{
    Time = 0,
    Reentry,
    None
}

public enum IS_AGGRESSIVE
{
    Peace = 0,
    SightAggressive,
    TerritoryAggressive,
    AttackAggressive,
    Boss
}

public enum IS_CHASE
{
    Peace = 0,
    Territory,
    AllTerritory
}

public enum IS_RUNAWAY
{
    Aggressive = 0,
    Peace,
    Coward
}
*/

public abstract class BasedMonster : MonoBehaviour
    // ��� ���� Ŭ���� (�߻� Ŭ����)
{
    // ���� ���Ϳ��� �ο��� ID
    private static int m_iNextValidID = 0;

    // ���� �ĺ� ��ȣ ID
    private int _id;
    public int ID // ������Ƽ
    {
        set
        {
            _id = value;
            m_iNextValidID++;
        }

        get => _id;
    }

    // ���� �̸�
    public string MonsterName { get; private set; } // �ڵ� ���� ������Ƽ

    // ü��
    private int _maxHp;
    public int CurHP { get; private set; } // �ڵ� ���� ������Ƽ

    // �� �� �Ӽ���
    public bool Dead { get; private set; } = false;
    public bool InAir { get; private set; } = false;

    // ���� �ʱ�ȭ
    public virtual void SetUp(string name, int maxHp) // ���� �Լ� ���
    {
        //Debug.Log("based");

        // ���� ��ȣ ����
        ID = m_iNextValidID;

        // �̸� ����
        MonsterName = name;

        // ü�� ����
        _maxHp = maxHp;
        CurHP = _maxHp;
    }

    // ������ �ǰ�
    public virtual void OnDamage(int _damage)
    {
        CurHP -= _damage;

        if (CurHP <= 0)
        {
            CurHP = 0;
            Die();
        }
    }
    
    public virtual void Die()
    {
        Dead = true;
    }
}
