using UnityEngine;

public enum BearAttackType
{
    Null = 0,

    // Normal Attack
    SlashRight,
    SlashLeft,
    BodySlam,
    Stomp,

    // Ultimate Attack
    EarthQuake = 10
}


public enum BlackPantherAttackType
{
    Null = 0,

    // Normal Attack
    VineMissile,
    VinePillar,

    // Ultimate Attack
    
}

[CreateAssetMenu(fileName = "BossAttackList", menuName = "Monster/BossAttackList", order = 1)]
public class BossAttackList : ScriptableObject
{
    // N개의 일반 스킬을 리스트로 관리한다
    // [SerializeField] private List<eachNormalSkill> normalSkillList;

    // N개의 궁극기 스킬을 리스트로 관리한다
    // [SerializeField] private List<eachUltimateSkill> ultimateSkillList;

    // 각각의 스킬을 클래스로 만들어 컴포넌트로써 활용한다
}
