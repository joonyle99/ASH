using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", order = 1)]
public class MonsterData : ScriptableObject
{
    // 몬스터 이름
    [SerializeField] private MonsterDefine.MonsterName _monsterName;
    public MonsterDefine.MonsterName MonsterName => _monsterName;

    // 몬스터 랭크
    [SerializeField] private MonsterDefine.RankType _rankType;
    public MonsterDefine.RankType RankType => _rankType;

    // 몬스터 행동 타입
    [SerializeField] private MonsterDefine.MoveType _moveType;
    public MonsterDefine.MoveType MoveType => _moveType;

    // 최대 체력
    [SerializeField] private int _maxHp;
    public int MaxHp => _maxHp;

    // 이동 속도
    [SerializeField] private float _moveSpeed;
    public float MoveSpeed => _moveSpeed;

    // 가속도
    [SerializeField] private float _acceleration;
    public float Acceleration => _acceleration;

    // 점프 파워
    [SerializeField] private Vector2 _jumpForce;
    public Vector2 JumpForce => _jumpForce;
}
