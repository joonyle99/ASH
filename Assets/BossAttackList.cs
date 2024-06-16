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
    // N���� �Ϲ� ��ų�� ����Ʈ�� �����Ѵ�
    // [SerializeField] private List<eachNormalSkill> normalSkillList;

    // N���� �ñر� ��ų�� ����Ʈ�� �����Ѵ�
    // [SerializeField] private List<eachUltimateSkill> ultimateSkillList;

    // ������ ��ų�� Ŭ������ ����� ������Ʈ�ν� Ȱ���Ѵ�
}
