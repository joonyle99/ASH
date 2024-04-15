using UnityEngine;

public class InstantRespawnOnContact : TriggerZone
{
    [SerializeField] readonly float _damage = 1;

    public override void OnPlayerEnter(PlayerBehaviour player)
    {
        player.TriggerInstantRespawn(_damage);
    }

    public override void OnActivatorEnter(TriggerActivator activator)
    {
        // Ȱ��ȭ�� ������Ʈ�� ���Ͷ��
        if (activator.Type == ActivatorType.Monster)
        {
            // �ź��̸� �����Ų��
            var turtle = activator.GetComponent<Turtle>();
            if (turtle) turtle.CurHp -= turtle.monsterData.MaxHp;
        }
    }
}
