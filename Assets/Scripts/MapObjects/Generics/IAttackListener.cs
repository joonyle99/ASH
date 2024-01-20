
using UnityEngine;

public enum AttackType
{
    BasicAttack,
    SkillAttack,
}
public class AttackInfo
{
    public float Damage = 1f;
    public Vector2 Force = Vector2.zero;
    public AttackType Type = AttackType.BasicAttack;

    // »ý¼ºÀÚ
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
