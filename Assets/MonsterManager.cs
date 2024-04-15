using System.Collections;
using System.Collections.Generic;
using joonyleTools;
using UnityEngine;

public class MonsterManager : SingletonBehavior<MonsterManager>
{
    // 씬에 존재하는 모든 몬스터 리스트
    // public List<MonsterBehavior> monsterList;

    private void Start()
    {
        // var monsterArray = FindObjectsOfType<MonsterBehavior>();
        // monsterList = new List<MonsterBehavior>(monsterArray);
    }

    public void NotifyDeath(MonsterBehavior monster, float reviveTime = 5f)
    {
        StartCoroutine(ReviveMonster(monster, reviveTime));
    }
    private IEnumerator ReviveMonster(MonsterBehavior monster, float reviveTime)
    {
        yield return new WaitForSeconds(reviveTime);
        monster.Revive();
    }
}
 