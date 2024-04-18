using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 지상 몬스터의 리스폰 데이터를 가진 구조체
/// </summary>
[Serializable]
public struct GroundRespawnData
{
    public Vector3 PatrolPointAPosition;
    public Vector3 PatrolPointBPosition;

    public Line RespawnLine;            // 두 점 사이의 임의의 점 위치에서 리스폰된다

    public GroundRespawnData(Vector3 patrolPointAPosition, Vector3 patrolPointBPosition, Line respawnLine)
    {
        PatrolPointAPosition = patrolPointAPosition;
        PatrolPointBPosition = patrolPointBPosition;

        RespawnLine = respawnLine;
    }
}

/// <summary>
/// 공중 몬스터의 리스폰 데이터를 가진 구조체
/// </summary>
[Serializable]
public struct FloatingRespawnData
{
    public NavMeshData NavMeshData;

    public Vector3 PatrolAreaPosition;
    public Vector3 ChaseAreaPosition;

    public Vector3 PatrolAreaScale;
    public Vector3 ChaseAreaScale;

    public Bounds RespawnBounds;        // 해당 영역의 랜덤 위치에서 리스폰된다

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
