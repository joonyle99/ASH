/// <summary>
/// �ν��Ͻ��� �������� �ʴ� Static Ŭ������ �����Ѵ�.
/// </summary>
public static class MonsterDefine
{
    public enum Name
    {
        Null = 0,

        �Ѥ�Normal�Ѥ�,

        // Normal
        ���� = 1001,
        ���ðź�,
        �밳����,
        ��������,
        ��������,
        ���罽����,
        �����׶���,
        �鰳,
        �˺ξ���,
        ��������,
        ������ĸ�,

        �Ѥ�SemiBoss�Ѥ�,

        // SemiBoss
        ��� = 2001,

        �Ѥ�FinalBoss�Ѥ�,

        // FinalBoss
        �� = 3001,

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
