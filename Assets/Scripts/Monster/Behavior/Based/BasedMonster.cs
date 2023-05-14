using UnityEditor.U2D.Path;
using UnityEngine;

#region Enum

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

#endregion

public class BasedMonster : MonsterBehaviour
    // ��� ���� Ŭ����
{
    #region Attribute

    // ���� �ĺ� ��ȣ ID
    public int ID { get; protected set; }

    // ���� �̸�
    public string MonsterName { get; protected set; }

    // ü��
    public int MaxHp { get; protected set; }
    public int CurHP { get; protected set; }

    // �̵��ӵ�
    public float MoveSpeed { get; protected set; }


    public bool IsReturn { get; protected set; }
    public SIZE Size { get; protected set; }
    public TYPE Type { get; protected set; }
    public ACTION_TYPE ActionType { get; protected set; }
    public RESPONE Response { get; protected set; }
    public IS_AGGRESSIVE IsAggressive { get; protected set; }
    public IS_CHASE IsChase { get; protected set; }
    public IS_RUNAWAY IsRunaway { get; protected set; }


    // �� �� �Ӽ���
    public bool Dead { get; protected set; } = false;
    public bool InAir { get; protected set; } = false;

    [SerializeField]
    private Rigidbody2D _rigidbody;
    
    public Rigidbody2D Rigidbody { get; protected set; }

    #endregion

    #region Function

    // ���� �ʱ�ȭ
    public virtual void SetUp(string name, int maxHp, TYPE type, ACTION_TYPE aType)
    {
        // �̸� ����
        MonsterName = name;

        // ü�� ����
        MaxHp = maxHp;
        CurHP = MaxHp;
    }

    // ������ �ǰ�
    public virtual void OnDamage(int damage)
    {
        CurHP -= damage;

        if (CurHP <= 0)
        {
            CurHP = 0;
            Die();
        }
    }

    public virtual void KnockBack(Vector2 vec)
    {

    }

    public virtual void Die()
    {
        Dead = true;
    }

    #endregion
}
