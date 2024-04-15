using UnityEngine;

public class Monster_BodySkill : Monster_Skill
{
    private MonsterBehavior _monster;

    void Awake()
    {
        _monster = GetComponentInParent<MonsterBehavior>();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (_monster.IsDead) return;

        base.OnTriggerEnter2D(collision);
    }
}
