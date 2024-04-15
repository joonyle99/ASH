using UnityEngine;

[CreateAssetMenu(fileName = "MonsterDataObject", order = 1)]
public class MonsterDataObject : ScriptableObject
{
    // ���� �̸�
    [field: SerializeField]
    public MonsterDefine.MonsterName Name { get; private set; }

    // ���� ��ũ
    [field: SerializeField]
    public MonsterDefine.RankType RankType { get; private set; }

    // ���� �ൿ Ÿ��
    [field: SerializeField]
    public MonsterDefine.MoveType MoveType { get; private set; }

    // �ִ� ü��
    [field: SerializeField]
    public int MaxHp { get; private set; }

    // �̵� �ӵ�
    [field: SerializeField]
    public float MoveSpeed { get; private set; }

    // ���ӵ�
    [field: SerializeField]
    public float Acceleration { get; private set; }

    // ���� �Ŀ�
    [field: SerializeField]
    public Vector2 JumpForce { get; private set; }
}
