using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Object/MonsterData", order = 1)]
public class MonsterData : ScriptableObject
{
    // 몬스터 이름
    [SerializeField] private string _monsterName;
    public string MonsterName => _monsterName;

    // 최대 체력
    [SerializeField] private int _maxHp;
    public int MaxHp => _maxHp;

    // 이동 속도
    [SerializeField] private float _moveSpeed;
    public float MoveSpeed => _moveSpeed;

    // 점프 파워
    [SerializeField] private Vector2 _jumpForce;
    public Vector2 JumpForce => _jumpForce;

    // 몬스터 행동 타입
    [SerializeField] private MonsterDefine.MONSTER_BEHAV _monsterBehav;
    public MonsterDefine.MONSTER_BEHAV MonsterBehav => _monsterBehav;
}
