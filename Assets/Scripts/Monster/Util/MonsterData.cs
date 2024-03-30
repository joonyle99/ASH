using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", order = 1)]
public class MonsterData : ScriptableObject
{
    // ���� �̸�
    [SerializeField] private MonsterDefine.MonsterName _monsterName;
    public MonsterDefine.MonsterName MonsterName => _monsterName;

    // ���� ��ũ
    [SerializeField] private MonsterDefine.RankType _rankType;
    public MonsterDefine.RankType RankType => _rankType;

    // ���� �ൿ Ÿ��
    [SerializeField] private MonsterDefine.MoveType _moveType;
    public MonsterDefine.MoveType MoveType => _moveType;

    // �ִ� ü��
    [SerializeField] private int _maxHp;
    public int MaxHp => _maxHp;

    // �̵� �ӵ�
    [SerializeField] private float _moveSpeed;
    public float MoveSpeed => _moveSpeed;

    // ���ӵ�
    [SerializeField] private float _acceleration;
    public float Acceleration => _acceleration;

    // ���� �Ŀ�
    [SerializeField] private Vector2 _jumpForce;
    public Vector2 JumpForce => _jumpForce;
}
