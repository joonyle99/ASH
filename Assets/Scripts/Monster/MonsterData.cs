using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Object/MonsterData", order = 1)]
public class MonsterData : ScriptableObject
{
    // ���� �̸�
    [SerializeField] private string _monsterName;
    public string MonsterName => _monsterName;

    // �ִ� ü��
    [SerializeField] private int _maxHp;
    public int MaxHp => _maxHp;

    // �̵� �ӵ�
    [SerializeField] private float _moveSpeed;
    public float MoveSpeed => _moveSpeed;

    // ���� �Ŀ�
    [SerializeField] private Vector2 _jumpForce;
    public Vector2 JumpForce => _jumpForce;

    // ���� �ൿ Ÿ��
    [SerializeField] private MonsterDefine.MONSTER_BEHAV _monsterBehav;
    public MonsterDefine.MONSTER_BEHAV MonsterBehav => _monsterBehav;
}
