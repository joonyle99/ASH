public class MonsterDefine
{
    public enum SIZE
    {
        Null = 0,
        Small,
        Medium,
        Large
    }

    public enum TYPE
    {
        Null = 0,
        Normal,
        SemiBoss,
        Boss
    }

    public enum ACTION
    {
        Null = 0,
        Ground,
        Floating,
        Crawl
    }

    public enum RESPONE
    {
        Null = 0,
        Time,
        Reentry
    }

    public enum AGGRESSIVE
    {
        Null = 0,
        Peace,
        SightAggressive,
        TerritoryAggressive,
        AttackAggressive,
        Boss
    }

    public enum CHASE
    {
        Null = 0,
        Peace,
        Territory,
        AllTerritory
    }

    public enum RUNAWAY
    {
        Null = 0,
        Aggressive,
        Peace,
        Coward
    }
}
