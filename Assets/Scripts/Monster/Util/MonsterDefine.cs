/// <summary>
/// �ν��Ͻ��� �������� �ʴ� Static Ŭ������ �����Ѵ�.
/// </summary>
public static class MonsterDefine
{
    public enum MonsterName
    {
        Null = 0,

        �Ѥ�Normal�Ѥ�,

        // Normal
        Bat = 1001,
        Turtle,
        Frog,
        Mushroom,

        �Ѥ�SemiBoss�Ѥ�,

        // SemiBoss
        Bear = 2001,

        �Ѥ�FinalBoss�Ѥ�,

        // FinalBoss
        Fire = 3001,

    }

    public enum RankType
    {
        Null = 0,
        Normal,
        SemiBoss,
        FinalBoss,
    }

    public enum MoveType
    {
        Null = 0,
        Ground,
        Fly
    }

    public static int BossHealthUnit = 10000;
}
