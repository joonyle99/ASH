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
    // ���� �����ϴ� ��� ���� ����Ʈ
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
            Debug.LogWarning("���� ���� ���Ͱ� �������� �ʽ��ϴ�");
            yield break;
        }

        if (!monsterList.Contains(monster))
        {
            Debug.LogWarning("���� ����Ʈ�� �����Ϸ��� ���Ͱ� �������� �ʽ��ϴ�");
            yield break;
        }

        // ������ ������ �����´�
        var monsterData = monster.monsterData;
        var respawnData = monster.respawnData;

        RemoveProcess(monster);

        yield return new WaitForSeconds(reviveTime);

        AddProcess(monsterData, respawnData);
    }
    private void RemoveProcess(MonsterBehavior monster)
    {
        // ���� ��Ͽ��� �����Ѵ�
        Debug.Log(monster.GetHashCode());
        monsterList.Remove(monster);

        // ���͸� �����Ѵ�
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

        // ���� ��Ͽ� �߰��Ѵ�
        monsterList.Add(newMonster);
        Debug.Log(newMonster.GetHashCode());
    }
    private Vector3 CalculateRespawnPosition(MonsterDefine.MoveType moveType, Vector3 firstPosition, Bounds respawnBounds)
    {
        var respawnPos = Vector3.zero;

        if (respawnBounds.size.x < 0.1f || respawnBounds.size.y < 0.1f)
        {
            Debug.LogWarning($"������ �ٿ�� ����� �ʹ� �۽��ϴ� x: {respawnBounds.size.x} / y: {respawnBounds.size.y}");

            // ������ �ٿ�� ����� �ʹ� ���� ��, ������ �ʱ� ��ġ�� �������ϵ��� �Ѵ�
            return firstPosition;
        }

        // ���� ������ ������ ��ġ ����
        if (moveType == MonsterDefine.MoveType.Ground)
        {
            var posX = Random.Range(respawnBounds.min.x, respawnBounds.max.x);
            var posY = firstPosition.y;
            var posZ = firstPosition.z;

            respawnPos = new Vector3(posX, posY, posZ);
        }
        // ���� ������ ������ ��ġ ����
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
