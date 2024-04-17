using System.Collections;
using System.Collections.Generic;
using joonyleTools;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class MonsterRespawnManager : SingletonBehavior<MonsterRespawnManager>
{
    // ���� �����ϴ� ��� ���� ����Ʈ
    public List<MonsterBehavior> monsterList;

    public Range respawnTimeRange;

    protected override void Awake()
    {
        base.Awake();

        // ��� ���͸� ã�Ƽ� ����Ʈ�� �߰��Ѵ�
        var monsterArray = FindObjectsOfType<MonsterBehavior>();
        monsterList = new List<MonsterBehavior>(monsterArray);
    }

    public void NotifyDeath(MonsterBehavior monster)
    {
        if (monster == null)
        {
            Debug.LogWarning("���� ���� ���Ͱ� �������� �ʽ��ϴ�");
            return;
        }

        if (!monsterList.Contains(monster))
        {
            Debug.LogWarning("���� ����Ʈ�� �����Ϸ��� ���Ͱ� �������� �ʽ��ϴ�");
            return;
        }

        StartCoroutine(RespawnCoroutine(monster, respawnTimeRange.Random()));
    }
    private IEnumerator RespawnCoroutine(MonsterBehavior monster, float respawnTime)
    {
        // ������ ������ �����´�
        var monsterData = monster.monsterData;
        var respawnData = monster.respawnData;

        RemoveProcess(monster);

        yield return new WaitForSeconds(respawnTime);

        AddProcess(monsterData, respawnData);
    }
    private void RemoveProcess(MonsterBehavior monster)
    {
        // ���� ��Ͽ��� �����Ѵ�
        Debug.Log(monster.GetHashCode());
        monsterList.Remove(monster);

        // ���͸� ������ �����Ѵ�
        monster.DestroyMonster();
    }
    private void AddProcess(MonsterBehavior.MonsterData monsterData, MonsterBehavior.RespawnData respawnData)
    {
        // ���͸� ���� �������� �ε��Ѵ�
        var resourcePath = "Prefabs/Monster/Normal/Prefab_" + monsterData.MonsterName;
        var prefabResource = Resources.Load<GameObject>(resourcePath);

        // � Ÿ���� �����̵�, ���͸� ���� �������� ������ ��ġ�� ���� �������� ��ġ�� �����ϴ�
        var prefab = Instantiate(prefabResource, respawnData.DefaultPrefabPosition, Quaternion.identity);

        // �������� ���͸� �����Ѵ�
        var actualMonster = prefab.GetComponentInChildren<MonsterBehavior>();

        // ������ ������ ��ġ�� �����Ѵ�
        var respawnPosition = CalculateRespawnPosition(monsterData.MoveType, respawnData);
        actualMonster.transform.position = respawnPosition;

        // ���� ������ ���μ��� ����
        actualMonster.RespawnProcess();

        // ���� ��Ͽ� �߰��Ѵ�
        monsterList.Add(actualMonster);
        Debug.Log(actualMonster.GetHashCode());
    }
    private Vector3 CalculateRespawnPosition(MonsterDefine.MoveType moveType, MonsterBehavior.RespawnData respawnData)
    {
        var respawnPos = Vector3.zero;

        // ���� ������ ������ ��ġ ����
        if (moveType == MonsterDefine.MoveType.Ground)
        {
            var t = Random.Range(0.0f, 1.0f); // 0�� 1 ������ ������ ��

            var posX = Mathf.Lerp(respawnData.RespawnLine.pointA.x, respawnData.RespawnLine.pointB.x, t);
            var posY = Mathf.Lerp(respawnData.RespawnLine.pointA.y, respawnData.RespawnLine.pointB.y, t);
            var posZ = respawnData.DefaultPrefabPosition.z;

            respawnPos = new Vector3(posX, posY, posZ);

            Debug.DrawRay(respawnPos, Vector3.up, Color.red, 3f);
        }
        // ���� ������ ������ ��ġ ����
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
