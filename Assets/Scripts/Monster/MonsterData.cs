using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Object/MonsterData", order = 1)]
public class MonsterData : ScriptableObject
{
    // ���� �̸�
    [SerializeField] private string _name;
    public string Name => _name;

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

    // ���� �ൿ Ÿ��
    [SerializeField] private MonsterDefine.MoveType _moveType;
    public MonsterDefine.MoveType MoveType => _moveType;
}
