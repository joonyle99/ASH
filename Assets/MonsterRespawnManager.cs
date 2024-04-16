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
        var respawnPos = Vector3.zero;

        if (monster.monsterData.MoveType == MonsterDefine.MoveType.Ground)
        {
            var posX = Random.Range(respawnBounds.min.x, respawnBounds.max.x);
            var posY = monster.FirstPosition.y;

            respawnPos = new Vector3(posX, posY, 0f);
        }
        else if (monster.monsterData.MoveType == MonsterDefine.MoveType.Fly)
        {
            var posX = Random.Range(respawnBounds.min.x, respawnBounds.max.x);
            var posY = Random.Range(respawnBounds.min.y, respawnBounds.max.y);

            respawnPos = new Vector3(posX, posY, 0f);
        }

        monster.Respawn(respawnPos);
    }
}
 