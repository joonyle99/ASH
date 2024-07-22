using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ���� ������ ������ �����͸� ���� ����ü
/// </summary>
[Serializable]
public struct GroundActionAreaData
{
    public Vector3 PatrolPointAPosition;
    public Vector3 PatrolPointBPosition;

    public joonyle99.Line3D respawnLine3D;            // �� �� ������ ������ �� ��ġ���� �������ȴ�

    public GroundActionAreaData(Vector3 patrolPointAPosition, Vector3 patrolPointBPosition, joonyle99.Line3D respawnLine3D)
    {
        PatrolPointAPosition = patrolPointAPosition;
        PatrolPointBPosition = patrolPointBPosition;

        this.respawnLine3D = respawnLine3D;
    }
}

/// <summary>
/// ���� ������ ������ �����͸� ���� ����ü
/// </summary>
[Serializable]
public struct FloatingActionAreaData
{
    public Vector3 PatrolAreaPosition;
    public Vector3 ChaseAreaPosition;

    public Vector3 PatrolAreaScale;
    public Vector3 ChaseAreaScale;

    public Bounds RespawnBounds;        // �ش� ������ ���� ��ġ���� �������ȴ�

    public FloatingActionAreaData(Vector3 patrolAreaPosition, Vector3 chaseAreaPosition, Vector3 patrolAreaScale, Vector3 chaseAreaScale, Bounds respawnBounds)
    {
        PatrolAreaPosition = patrolAreaPosition;
        ChaseAreaPosition = chaseAreaPosition;

        PatrolAreaScale = patrolAreaScale;
        ChaseAreaScale = chaseAreaScale;

        RespawnBounds = respawnBounds;
    }
}

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
        Bat2,
        Turtle,
        Turtle2,
        Frog,
        Frog2,
        Mushroom,
        Mushroom2,

        �Ѥ�SemiBoss�Ѥ�,

        // SemiBoss
        Bear = 2001,
        BlackPanther,

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
        GroundNormal,
        GroundTurret,
        FloatingNormal,
        FloatingTurret,
    }

    public static int BossHealthUnit = 10000;
}
