/// <summary>
/// 인스턴스를 생성하지 않는 Static 클래스로 제작한다.
/// </summary>
public static class MonsterDefine
{
    public enum SIZE
    {
        Null = 0,
        Small,
        Medium,
        Large
    }

    public enum MONSTER_TYPE
    {
        Null = 0,
        Normal,
        SemiBoss,
        Boss
    }
}
