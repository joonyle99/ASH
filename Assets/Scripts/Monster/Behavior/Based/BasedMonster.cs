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
/// 몬스터의 기본 행동을 정의
/// </summary>
public abstract class BasedMonster : MonsterBehaviour
{
#region Attribute

    // 고유 식별 번호 ID
    public int ID { get; protected set; }

    // 몬스터 이름
    public string MonsterName { get; protected set; }

    // 최대 체력
    public int MaxHp { get; protected set; }

    // 현재 체력
    public int CurHP { get; protected set; }

    // 이동속도
    public float MoveSpeed { get; protected set; }


    // 추가 프로퍼티
    public SIZE Size { get; protected set; } // 몬스터 크기 구분
    public TYPE Type { get; protected set; } // 몬스터 종류
    public ACTION_TYPE ActionType { get; protected set; } // 몬스터 활동 종류
    public RESPONE Response { get; protected set; } // 리젠 방식 구분
    public bool IsReturn { get; protected set; } // 귀환 여부
    public IS_AGGRESSIVE IsAggressive { get; protected set; } // 선공 여부
    public IS_CHASE IsChase { get; protected set; } // 추경 방식 구분
    public IS_RUNAWAY IsRunaway { get; protected set; } // 도망 여부


    // 그 외 속성들
    public bool Dead { get; protected set; } = false;
    public bool InAir { get; protected set; } = false;

    #endregion

    #region Function

    // 몬스터 초기화
    public abstract void SetUp();

    // 데미지 피격
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

    // 넉백
    public abstract void KnockBack(Vector2 vec);

    // 사망
    public virtual void Die()
    {
        // 충돌 비활성화
        gameObject.GetComponent<Collider2D>().enabled = false;

        Dead = true;
    }


    #endregion
}
