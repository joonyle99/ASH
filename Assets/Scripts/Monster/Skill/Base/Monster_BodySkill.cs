using UnityEngine;

/// <summary>
/// ������ ��ü �� �Ϻη� �����ϴ� ��ų�̴�.
/// �ߵ� ��, �������� ������ ��Ȱ��ȭ �Ǵ� ����Ǵ� ��ų
/// </summary>
public class Monster_BodySkill : Monster_Skill
{
    protected override void Awake()
    {
        base.Awake();

        // BodySkill�� ��� Actor�� �ڵ����� �������ش�
        actor = GetComponent<MonsterBehaviour>() ?? GetComponentInParent<MonsterBehaviour>();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (actor == null)
        {
            Debug.LogError("BodySkill�� Actor�� �����ϴ�");
            return;
        }

        if (actor.IsDead)
        {
            Debug.Log("BodySkill�� Actor�� ��� �����Դϴ�");
            return;
        }

        base.OnTriggerEnter2D(collision);
    }
}
