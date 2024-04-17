using System.Collections;
using System.Collections.Generic;
using joonyleTools;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class MonsterRespawnManager : SingletonBehavior<MonsterRespawnManager>
{
    // 씬에 존재하는 모든 몬스터 리스트
    public List<MonsterBehavior> monsterList;

    public Range respawnTimeRange;

    protected override void Awake()
    {
        base.Awake();

        // 모든 몬스터를 찾아서 리스트에 추가한다
        var monsterArray = FindObjectsOfType<MonsterBehavior>();
        monsterList = new List<MonsterBehavior>(monsterArray);
    }

    public void NotifyDeath(MonsterBehavior monster)
    {
        if (monster == null)
        {
            Debug.LogWarning("전달 받은 몬스터가 존재하지 않습니다");
            return;
        }

        if (!monsterList.Contains(monster))
        {
            Debug.LogWarning("몬스터 리스트에 제거하려는 몬스터가 존재하지 않습니다");
            return;
        }

        StartCoroutine(RespawnCoroutine(monster, respawnTimeRange.Random()));
    }
    private IEnumerator RespawnCoroutine(MonsterBehavior monster, float respawnTime)
    {
        // 몬스터의 정보를 가져온다
        var monsterData = monster.monsterData;
        var respawnData = monster.respawnData;

        RemoveProcess(monster);

        yield return new WaitForSeconds(respawnTime);

        AddProcess(monsterData, respawnData);
    }
    private void RemoveProcess(MonsterBehavior monster)
    {
        // 몬스터 목록에서 제거한다
        Debug.Log(monster.GetHashCode());
        monsterList.Remove(monster);

        // 몬스터를 씬에서 삭제한다
        monster.DestroyMonster();
    }
    private void AddProcess(MonsterBehavior.MonsterData monsterData, MonsterBehavior.RespawnData respawnData)
    {
        // 몬스터를 담은 프리팹을 로드한다
        var resourcePath = "Prefabs/Monster/Normal/Prefab_" + monsterData.MonsterName;
        var prefabResource = Resources.Load<GameObject>(resourcePath);

        // 어떤 타입의 몬스터이든, 몬스터를 담은 프리팹의 리스폰 위치는 이전 프리팹의 위치와 동일하다
        var prefab = Instantiate(prefabResource, respawnData.DefaultPrefabPosition, Quaternion.identity);

        // 실질적인 몬스터를 추출한다
        var actualMonster = prefab.GetComponentInChildren<MonsterBehavior>();

        // 몬스터의 리스폰 위치를 변경한다
        var respawnPosition = CalculateRespawnPosition(monsterData.MoveType, respawnData);
        actualMonster.transform.position = respawnPosition;

        // 몬스터 리스폰 프로세스 실행
        actualMonster.RespawnProcess();

        // 몬스터 목록에 추가한다
        monsterList.Add(actualMonster);
        Debug.Log(actualMonster.GetHashCode());
    }
    private Vector3 CalculateRespawnPosition(MonsterDefine.MoveType moveType, MonsterBehavior.RespawnData respawnData)
    {
        var respawnPos = Vector3.zero;

        // 지형 몬스터의 리스폰 위치 설정
        if (moveType == MonsterDefine.MoveType.Ground)
        {
            var t = Random.Range(0.0f, 1.0f); // 0과 1 사이의 임의의 수

            var posX = Mathf.Lerp(respawnData.RespawnLine.pointA.x, respawnData.RespawnLine.pointB.x, t);
            var posY = Mathf.Lerp(respawnData.RespawnLine.pointA.y, respawnData.RespawnLine.pointB.y, t);
            var posZ = respawnData.DefaultPrefabPosition.z;

            respawnPos = new Vector3(posX, posY, posZ);

            Debug.DrawRay(respawnPos, Vector3.up, Color.red, 3f);
        }
        // 비행 몬스터의 리스폰 위치 설정
        else if (moveType == MonsterDefine.MoveType.Fly)
        {
            var respawnBounds = respawnData.RespawnBounds;

            var posX = Random.Range(respawnBounds.min.x, respawnBounds.max.x);
            var posY = Random.Range(respawnBounds.min.y, respawnBounds.max.y);
            var posZ = respawnData.DefaultPrefabPosition.z;

            respawnPos = new Vector3(posX, posY, posZ);
        }

        return respawnPos;
    }
}
