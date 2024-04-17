/// <summary>
/// 인스턴스를 생성하지 않는 Static 클래스로 제작한다.
/// </summary>
public static class MonsterDefine
{
    public enum MonsterName
    {
        Null = 0,

        ㅡㅡNormalㅡㅡ,

        // Normal
        Bat = 1001,
        Turtle,
        Frog,
        Mushroom,

        ㅡㅡSemiBossㅡㅡ,

        // SemiBoss
        Bear = 2001,

        ㅡㅡFinalBossㅡㅡ,

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
