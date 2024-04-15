using UnityEngine;

[CreateAssetMenu(fileName = "MonsterDataObject", order = 1)]
public class MonsterDataObject : ScriptableObject
{
    // 몬스터 이름
    [field: SerializeField]
    public MonsterDefine.MonsterName Name { get; private set; }

    // 몬스터 랭크
    [field: SerializeField]
    public MonsterDefine.RankType RankType { get; private set; }

    // 몬스터 행동 타입
    [field: SerializeField]
    public MonsterDefine.MoveType MoveType { get; private set; }

    // 최대 체력
    [field: SerializeField]
    public int MaxHp { get; private set; }

    // 이동 속도
    [field: SerializeField]
    public float MoveSpeed { get; private set; }

    // 가속도
    [field: SerializeField]
    public float Acceleration { get; private set; }

    // 점프 파워
    [field: SerializeField]
    public Vector2 JumpForce { get; private set; }
}
