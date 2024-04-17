using UnityEngine;

public class Monster_BodySkill : Monster_Skill
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (monster.IsDead) return;

        base.OnTriggerEnter2D(collision);
    }
}
