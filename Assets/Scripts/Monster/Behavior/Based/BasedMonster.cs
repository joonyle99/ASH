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

/// <summary>
/// ������ �⺻ �ൿ�� ����
/// </summary>
public abstract class BasedMonster : MonsterBehaviour
{
#region Attribute

    // ���� �ĺ� ��ȣ ID
    public int ID { get; protected set; }

    // ���� �̸�
    public string MonsterName { get; protected set; }

    // �ִ� ü��
    public int MaxHp { get; protected set; }

    // ���� ü��
    public int CurHP { get; protected set; }

    // �̵��ӵ�
    public float MoveSpeed { get; protected set; }


    // �߰� ������Ƽ
    public SIZE Size { get; protected set; } // ���� ũ�� ����
    public TYPE Type { get; protected set; } // ���� ����
    public ACTION_TYPE ActionType { get; protected set; } // ���� Ȱ�� ����
    public RESPONE Response { get; protected set; } // ���� ��� ����
    public bool IsReturn { get; protected set; } // ��ȯ ����
    public IS_AGGRESSIVE IsAggressive { get; protected set; } // ���� ����
    public IS_CHASE IsChase { get; protected set; } // �߰� ��� ����
    public IS_RUNAWAY IsRunaway { get; protected set; } // ���� ����


    // �� �� �Ӽ���
    public bool Dead { get; protected set; } = false;
    public bool InAir { get; protected set; } = false;

    #endregion

    #region Function

    // ���� �ʱ�ȭ
    public abstract void SetUp();

    // ������ �ǰ�
    public virtual void OnDamage(int damage)
    {
        //Debug.Log("based ondamage");
        CurHP -= damage;

        if (CurHP <= 0)
        {
            CurHP = 0;
            Die();
        }
    }

    // �˹�
    public abstract void KnockBack(Vector2 vec);

    // ���
    public virtual void Die()
    {
        // �浹 ��Ȱ��ȭ
        gameObject.GetComponent<Collider2D>().enabled = false;

        Dead = true;
    }


    #endregion
}
