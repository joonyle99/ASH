using UnityEngine;

public enum AttackType
{
    Player_BasicAttack,
    Player_SkillAttack,
    Monster_BodyAttack,
    Monster_BasicAttack,
    Monster_SkillAttack,

    GimmickAttack,
}

public class AttackInfo
{
    public float Damage;
    public Vector2 Force;
    public AttackType Type;

    // »ý¼ºÀÚ
    public AttackInfo()
    {
        Damage = 1f;
        Force = Vector2.zero;
        Type = AttackType.Player_BasicAttack;
    }
    public AttackInfo(float damage, Vector2 force, AttackType type)
    {
        Damage = damage;
        Force = force;
        Type = type;
    }
}

public interface IAttackListener
{
    enum AttackResult { Fail, Success }
    public AttackResult OnHit(AttackInfo attackInfo);
}
