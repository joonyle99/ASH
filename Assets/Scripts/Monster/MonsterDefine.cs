/// <summary>
/// 인스턴스를 생성하지 않는 Static 클래스로 제작한다.
/// </summary>
public static class MonsterDefine
{
    public enum MoveType
    {
        Null = 0,
        Turret,
        GroundWalking,
        GroundJumpping,
        GroundFloating,
        Fly,
    }
}
