using System.Collections;
using System.Collections.Generic;
using joonyleTools;
using UnityEngine;

public class MonsterRespawnManager : SingletonBehavior<MonsterRespawnManager>
{
    // 씬에 존재하는 모든 몬스터 리스트
    public List<MonsterBehavior> monsterList;

    public Range respawnTimeRange;

    protected override void Awake()
    {
        base.Awake();

        var monsterArray = FindObjectsOfType<MonsterBehavior>();
        monsterList = new List<MonsterBehavior>(monsterArray);
    }

    public void NotifyDeath(MonsterBehavior monster)
    {
        StartCoroutine(RespawnMonster(monster, respawnTimeRange.Random()));
    }
    private IEnumerator RespawnMonster(MonsterBehavior monster, float reviveTime)
    {
        yield return new WaitForSeconds(reviveTime);

        var respawnBounds = monster.RespawnBounds;
        var respawnPos = new Vector3(Random.Range(respawnBounds.min.x, respawnBounds.max.x),
            Random.Range(respawnBounds.min.y, respawnBounds.max.y), 0f);

        monster.Respawn(respawnPos);
    }
}
 