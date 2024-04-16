using UnityEngine;

public class Monster_BodySkill : Monster_Skill
{
    private MonsterBehavior _monster;

    private void Awake()
    {
        _monster = GetComponentInParent<MonsterBehavior>();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (_monster.IsDead) return;

        base.OnTriggerEnter2D(collision);
    }

    /// <summary>
    /// ��ų ��� ���߿��� Active�Ǵ� BodySkill�� ���Ͱ� ���ڱ� �׾ ��ó ���� ������ ���� ����Ͽ�
    /// �ش� ���� ������Ʈ�� ��Ȱ��ȭ����� �Ѵ�.
    /// </summary>
    private void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
