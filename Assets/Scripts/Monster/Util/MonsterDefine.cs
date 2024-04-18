using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ���� ������ ������ �����͸� ���� ����ü
/// </summary>
[Serializable]
public struct GroundRespawnData
{
    public Vector3 PatrolPointAPosition;
    public Vector3 PatrolPointBPosition;

    public Line RespawnLine;            // �� �� ������ ������ �� ��ġ���� �������ȴ�

    public GroundRespawnData(Vector3 patrolPointAPosition, Vector3 patrolPointBPosition, Line respawnLine)
    {
        PatrolPointAPosition = patrolPointAPosition;
        PatrolPointBPosition = patrolPointBPosition;

        RespawnLine = respawnLine;
    }
}

/// <summary>
/// ���� ������ ������ �����͸� ���� ����ü
/// </summary>
[Serializable]
public struct FloatingRespawnData
{
    public NavMeshData NavMeshData;

    public Vector3 PatrolAreaPosition;
    public Vector3 ChaseAreaPosition;

    public Vector3 PatrolAreaScale;
    public Vector3 ChaseAreaScale;

    public Bounds RespawnBounds;        // �ش� ������ ���� ��ġ���� �������ȴ�

    public FloatingRespawnData(NavMeshData navMeshData, Vector3 patrolAreaPosition, Vector3 chaseAreaPosition, Vector3 patrolAreaScale, Vector3 chaseAreaScale, Bounds respawnBounds)
    {
        NavMeshData = navMeshData;

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
