/// <summary>
/// 인스턴스를 생성하지 않는 Static 클래스로 제작한다.
/// </summary>
public static class MonsterDefine
{
    public enum Name
    {
        Null = 0,

        ㅡㅡNormalㅡㅡ,

        // Normal
        박쥐 = 1001,
        가시거북,
        흙개구리,
        물개구리,
        껍질버섯,
        종양슬라임,
        슬러그라임,
        들개,
        늪부엉이,
        진흙전갈,
        박쥐공파리,

        ㅡㅡSemiBossㅡㅡ,

        // SemiBoss
        흑곰 = 2001,

        ㅡㅡFinalBossㅡㅡ,

        // FinalBoss
        불 = 3001,

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
        GroundTurret,
        GroundWalking,
        GroundJumpping,
        GroundFloating,
        Fly,
    }
}
