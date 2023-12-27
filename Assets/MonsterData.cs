using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Object/MonsterData", order = 1)]
public class MonsterData : ScriptableObject
{
    // ���� �ĺ� ��ȣ ID
    [SerializeField] private int _id;
    public int ID
    {
        get => _id;
        protected set => _id = value;
    }

    // ���� �̸�
    [SerializeField] private string _monsterName;
    public string MonsterName
    {
        get => _monsterName;
        protected set => _monsterName = value;
    }

    // �ִ� ü��
    [SerializeField] private int _maxHp;
    public int MaxHp
    {
        get => _maxHp;
        protected set => _maxHp = value;
    }

    // �̵��ӵ�
    [SerializeField] private float _moveSpeed;
    public float MoveSpeed
    {
        get => _moveSpeed;
        protected set => _moveSpeed = value;
    }

    // �߰� ������Ƽ
    [SerializeField] private MONSTER_SIZE _monsterSize;
    public MONSTER_SIZE MonsterSize // ���� ũ�� ����
    {
        get => _monsterSize;
        protected set => _monsterSize = value;
    }

    [SerializeField] private MONSTER_TYPE _monsterType;
    public MONSTER_TYPE MonsterType // ���� ����
    {
        get => _monsterType;
        protected set => _monsterType = value;
    }

    [SerializeField] private ACTION_TYPE _actionType;
    public ACTION_TYPE ActionType // ���� Ȱ�� ����
    {
        get => _actionType;
        protected set => _actionType = value;
    }

    [SerializeField] private RESPONE_TYPE _responseType;
    public RESPONE_TYPE ResponseType // ���� ��� ����
    {
        get => _responseType;
        protected set => _responseType = value;
    }

    [SerializeField] private AGGRESSIVE_TYPE _aggressiveType;
    public AGGRESSIVE_TYPE AggressiveType // ���� ����
    {
        get => _aggressiveType;
        protected set => _aggressiveType = value;
    }

    [SerializeField] private CHASE_TYPE _chaseType;
    public CHASE_TYPE ChaseType // ���� ��� ����
    {
        get => _chaseType;
        protected set => _chaseType = value;
    }

    [SerializeField] private RUNAWAY_TYPE _runawayType;
    public RUNAWAY_TYPE RunawayType // ���� ����
    {
        get => _runawayType;
        protected set => _runawayType = value;
    }
}
