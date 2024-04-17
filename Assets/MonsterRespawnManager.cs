using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using joonyleTools;
using UnityEngine;
using static MonsterBehavior;
using Random = UnityEngine.Random;

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
        if (monster == null)
        {
            Debug.LogWarning("전달 받은 몬스터가 존재하지 않습니다");
            yield break;
        }

        if (!monsterList.Contains(monster))
        {
            Debug.LogWarning("몬스터 리스트에 제거하려는 몬스터가 존재하지 않습니다");
            yield break;
        }

        // 몬스터의 정보를 가져온다
        var monsterData = monster.monsterData;
        var respawnData = monster.respawnData;

        RemoveProcess(monster);

        yield return new WaitForSeconds(reviveTime);

        AddProcess(monsterData, respawnData);
    }
    private void RemoveProcess(MonsterBehavior monster)
    {
        // 몬스터 목록에서 제거한다
        Debug.Log(monster.GetHashCode());
        monsterList.Remove(monster);

        // 몬스터를 제거한다
        monster.DestroyMonster();
    }
    private void AddProcess(MonsterData monsterData, RespawnData respawnData)
    {
        var resourcePath = "Prefabs/Monster/Normal/Prefab_" + monsterData.MonsterName;
        var prefab = Resources.Load<GameObject>(resourcePath);

        var respawnPosition = CalculateRespawnPosition(monsterData.MoveType, respawnData.FirstPosition, respawnData.RespawnBounds);

        var newInstance = Instantiate(prefab, respawnPosition, Quaternion.identity);
        var newMonster = newInstance.GetComponentInChildren<MonsterBehavior>();
        // var newMonster = newInstance.transform.GetChild(0).GetComponent<MonsterBehavior>();

        newMonster.Respawn();

        // 몬스터 목록에 추가한다
        monsterList.Add(newMonster);
        Debug.Log(newMonster.GetHashCode());
    }
    private Vector3 CalculateRespawnPosition(MonsterDefine.MoveType moveType, Vector3 firstPosition, Bounds respawnBounds)
    {
        var respawnPos = Vector3.zero;

        if (respawnBounds.size.x < 0.1f || respawnBounds.size.y < 0.1f)
        {
            Debug.LogWarning($"리스폰 바운드 사이즈가 너무 작습니다 x: {respawnBounds.size.x} / y: {respawnBounds.size.y}");

            // 리스폰 바운드 사이즈가 너무 작을 때, 몬스터의 초기 위치로 리스폰하도록 한다
            return firstPosition;
        }

        // 지형 몬스터의 리스폰 위치 설정
        if (moveType == MonsterDefine.MoveType.Ground)
        {
            var posX = Random.Range(respawnBounds.min.x, respawnBounds.max.x);
            var posY = firstPosition.y;
            var posZ = firstPosition.z;

            respawnPos = new Vector3(posX, posY, posZ);
        }
        // 비행 몬스터의 리스폰 위치 설정
        else if (moveType == MonsterDefine.MoveType.Fly)
        {
            var posX = Random.Range(respawnBounds.min.x, respawnBounds.max.x);
            var posY = Random.Range(respawnBounds.min.y, respawnBounds.max.y);
            var posZ = firstPosition.z;

            respawnPos = new Vector3(posX, posY, posZ);
        }

        return respawnPos;
    }
}
