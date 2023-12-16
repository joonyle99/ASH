using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Object/MonsterData", order = 1)]
public class MonsterData : ScriptableObject
{
    // 고유 식별 번호 ID
    [SerializeField] private int _id;
    public int ID
    {
        get => _id;
        protected set => _id = value;
    }

    // 몬스터 이름
    [SerializeField] private string _monsterName;
    public string MonsterName
    {
        get => _monsterName;
        protected set => _monsterName = value;
    }

    // 최대 체력
    [SerializeField] private int _maxHp;
    public int MaxHp
    {
        get => _maxHp;
        protected set => _maxHp = value;
    }

    // 이동속도
    [SerializeField] private float _moveSpeed;
    public float MoveSpeed
    {
        get => _moveSpeed;
        protected set => _moveSpeed = value;
    }

    // 추가 프로퍼티
    [SerializeField] private MONSTER_SIZE _monsterSize;
    public MONSTER_SIZE MonsterSize // 몬스터 크기 구분
    {
        get => _monsterSize;
        protected set => _monsterSize = value;
    }

    [SerializeField] private MONSTER_TYPE _monsterType;
    public MONSTER_TYPE MonsterType // 몬스터 종류
    {
        get => _monsterType;
        protected set => _monsterType = value;
    }

    [SerializeField] private ACTION_TYPE _actionType;
    public ACTION_TYPE ActionType // 몬스터 활동 종류
    {
        get => _actionType;
        protected set => _actionType = value;
    }

    [SerializeField] private RESPONE_TYPE _responseType;
    public RESPONE_TYPE ResponseType // 리젠 방식 구분
    {
        get => _responseType;
        protected set => _responseType = value;
    }

    [SerializeField] private AGGRESSIVE_TYPE _aggressiveType;
    public AGGRESSIVE_TYPE AggressiveType // 선공 여부
    {
        get => _aggressiveType;
        protected set => _aggressiveType = value;
    }

    [SerializeField] private CHASE_TYPE _chaseType;
    public CHASE_TYPE ChaseType // 추적 방식 구분
    {
        get => _chaseType;
        protected set => _chaseType = value;
    }

    [SerializeField] private RUNAWAY_TYPE _runawayType;
    public RUNAWAY_TYPE RunawayType // 도망 여부
    {
        get => _runawayType;
        protected set => _runawayType = value;
    }
}
