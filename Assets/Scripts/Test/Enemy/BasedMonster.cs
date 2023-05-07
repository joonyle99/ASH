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
    // 기반 몬스터 클래스 (추상 클래스)
{
    // 다음 몬스터에게 부여될 ID
    private static int m_iNextValidID = 0;

    // 고유 식별 번호 ID
    private int _id;
    public int ID // 프로퍼티
    {
        set
        {
            _id = value;
            m_iNextValidID++;
        }

        get => _id;
    }

    // 몬스터 이름
    public string MonsterName { get; private set; } // 자동 구현 프로퍼티

    // 체력
    private int _maxHp;
    public int CurHP { get; private set; } // 자동 구현 프로퍼티

    // 그 외 속성들
    public bool Dead { get; private set; } = false;
    public bool InAir { get; private set; } = false;

    // 몬스터 초기화
    public virtual void SetUp(string name, int maxHp) // 가상 함수 사용
    {
        //Debug.Log("based");

        // 고유 번호 설정
        ID = m_iNextValidID;

        // 이름 설정
        MonsterName = name;

        // 체력 설정
        _maxHp = maxHp;
        CurHP = _maxHp;
    }

    // 데미지 피격
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
