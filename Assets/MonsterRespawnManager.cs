using System.Collections;
using System.Collections.Generic;
using joonyleTools;
using UnityEngine;

public class MonsterRespawnManager : SingletonBehavior<MonsterRespawnManager>
{
    // ���� �����ϴ� ��� ���� ����Ʈ
    public List<MonsterBehavior> MonsterList;

    public Range RespawnTimeRange;

    protected override void Awake()
    {
        base.Awake();

        // ��� ���͸� ã�´�
        var monsterArray = FindObjectsOfType<MonsterBehavior>();

        // ����Ʈ�� �߰��Ѵ�
        MonsterList = new List<MonsterBehavior>(monsterArray);
    }

    public void NotifyDeath(MonsterBehavior monster)
    {
        if (!MonsterList.Contains(monster))
        {
            Debug.LogWarning("���� ����Ʈ�� �����Ϸ��� ���Ͱ� �������� �ʽ��ϴ�");
            return;
        }

        StartCoroutine(RespawnCoroutine(monster, RespawnTimeRange.Random()));
    }
    private IEnumerator RespawnCoroutine(MonsterBehavior monster, float respawnTime)
    {
        // �����͸� �����´�
        var monsterData = monster.monsterData;
        var respawnData = monster.respawnData;

        RemoveProcess(monster);

        yield return new WaitForSeconds(respawnTime);

        AddProcess(monsterData, respawnData);
    }
    private void RemoveProcess(MonsterBehavior monster)
    {
        // ���� ��Ͽ��� �����Ѵ�
        //Debug.Log($"removed monster hash code: {monster.GetHashCode()}");
        MonsterList.Remove(monster);

        // ���͸� ������ �����Ѵ�
        monster.DestroyMonster();
    }
    private void AddProcess(MonsterBehavior.MonsterData monsterData, MonsterBehavior.RespawnData respawnData)
    {
        // ������(���͸� ����)�� �ε��Ѵ�
        var resourcePath = "Prefabs/Monster/Normal/Prefab_" + monsterData.MonsterName;
        var prefabResource = Resources.Load<GameObject>(resourcePath);

        // Debug.Log("�ν��Ͻ� ���� �ٷ� ����");

        // � Ÿ���� �����̵�, ���͸� ���� �������� ������ ��ġ�� ���� �������� ��ġ�� �����ϴ�
        // ���ο� �ν��Ͻ��� �����Ǹ� ��� Awake() �޼ҵ尡 ȣ��. �� Instantiate ȣ���� �Ϸ���ڸ��� �ٷ� ����
        // �ش� ���� ������Ʈ�� ó�� Ȱ��ȭ�ǰ� �� ���� �����ӿ��� ó������ Start() �޼ҵ� ����. ��, �ν��Ͻ��� ������ �� ó������ �� �������� ������ �� ȣ��
        var prefabInstance = Instantiate(prefabResource, respawnData.DefaultPrefabPosition, Quaternion.identity);

        // Debug.Log("�ν��Ͻ� ���� �ٷ� ����");

        // �ൿ �ݰ� ������ �����Ѵ�
        var respawnDataSender = prefabInstance.GetComponentInChildren<RespawnDataSender>();
        // �������� ���͸� �����Ѵ�
        var monster = prefabInstance.GetComponentInChildren<MonsterBehavior>();

        // * �ൿ �ݰ� �����͸� �̿��� ������ �ൿ �ݰ� ������ ���� *
        SetActionAreaInfo(respawnDataSender, monsterData.MoveType, respawnData);
        // * ������ ��ġ �����͸� �̿��� ������ ��ġ�� ���� *
        SetMonsterRespawnPosition(monster.transform, monsterData.MoveType, respawnData);

        // ���� ������ ���μ��� ����
        monster.RespawnProcess();

        // ���� ��Ͽ� �߰��Ѵ�
        MonsterList.Add(monster);
        //Debug.Log($"added monster hash code: {monster.GetHashCode()}");
    }
    private void SetActionAreaInfo(RespawnDataSender respawnDataSender, MonsterDefine.MoveType moveType, MonsterBehavior.RespawnData respawnData)
    {
        // ���� ������ �ൿ �ݰ� ���� ����
        if (moveType == MonsterDefine.MoveType.Ground)
        {
            // �ൿ �ݰ� ������ �����´�
            respawnDataSender.ExtractActionAreaInfo(out var patrolPointA, out var patrolPointB);

            // ���ο� �ൿ �ݰ� ������ ������ ���� ���� �����

            // ��ġ ����
            patrolPointA.transform.position = respawnData.groundRespawnData.PatrolPointAPosition;
            patrolPointB.transform.position = respawnData.groundRespawnData.PatrolPointBPosition;

            // transform�� ��������� ��� ���� ������ �ݿ��Ǿ�, bounds�� transform�� ����ȭ�ȴ�
            Physics2D.SyncTransforms();

            var groundRespawnDataSender = respawnDataSender as GroundRespawnDataSender;
            if (groundRespawnDataSender == null)
            {
                Debug.LogError("GroundRespawnDataSender �����ϴ�");
                return;
            }

            // ����� ������ ������ ���Ӱ� ������ ��ü�� �Ҵ����ش�
            groundRespawnDataSender.UpdateRespawnData();
        }
        // ���� ������ �ൿ �ݰ� ���� ����
        else if (moveType == MonsterDefine.MoveType.Fly)
        {
            // �ൿ �ݰ� ������ �����´�
            respawnDataSender.ExtractActionAreaInfo(out var patrolArea, out var chaseArea);

            // ���ο� �ൿ �ݰ� ������ ������ ���� ���� �����

            // ��ġ ����
            patrolArea.transform.position = respawnData.floatingRespawnData.PatrolAreaPosition;
            chaseArea.transform.position = respawnData.floatingRespawnData.ChaseAreaPosition;

            // ũ�� ����
            patrolArea.transform.localScale = respawnData.floatingRespawnData.PatrolAreaScale;
            chaseArea.transform.localScale = respawnData.floatingRespawnData.ChaseAreaScale;

            // transform�� ��������� ��� ���� ������ �ݿ��Ǿ�, bounds�� transform�� ����ȭ�ȴ�
            Physics2D.SyncTransforms();

            var floatingRespawnDataSender = respawnDataSender as FloatingRespawnDataSender;
            if (floatingRespawnDataSender == null)
            {
                Debug.LogError("FloatingRespawnDataSender�� �����ϴ�");
                return;
            }

            // ������ NavMesh �����͸� �����Ѵ�
            floatingRespawnDataSender.SetNavMeshData(respawnData.floatingRespawnData.NavMeshData);

            // ����� ������ ������ ���Ӱ� ������ ��ü�� �Ҵ����ش�
            floatingRespawnDataSender.UpdateRespawnData();

            // ����� �ൿ �ݰ� ������ ��Ÿ�ӿ� NavMesh ����
            floatingRespawnDataSender.BakeNavMesh();
        }
    }
    private void SetMonsterRespawnPosition(Transform monsterTransform, MonsterDefine.MoveType moveType, MonsterBehavior.RespawnData respawnData)
    {
        var respawnPos = Vector3.zero;

        // ���� ������ ������ ��ġ ����
        if (moveType == MonsterDefine.MoveType.Ground)
        {
            var t = Random.Range(0.0f, 1.0f); // 0�� 1 ������ ������ ��

            var posX = Mathf.Lerp(respawnData.groundRespawnData.RespawnLine.pointA.x, respawnData.groundRespawnData.RespawnLine.pointB.x, t);
            var posY = Mathf.Lerp(respawnData.groundRespawnData.RespawnLine.pointA.y, respawnData.groundRespawnData.RespawnLine.pointB.y, t);
            var posZ = respawnData.DefaultPrefabPosition.z;

            respawnPos = new Vector3(posX, posY, posZ);

            Debug.DrawRay(respawnPos, Vector3.up, Color.red, 3f);
        }
        // ���� ������ ������ ��ġ ����
        else if (moveType == MonsterDefine.MoveType.Fly)
        {
            var respawnBounds = respawnData.floatingRespawnData.RespawnBounds;

            var posX = Random.Range(respawnBounds.min.x, respawnBounds.max.x);
            var posY = Random.Range(respawnBounds.min.y, respawnBounds.max.y);
            var posZ = respawnData.DefaultPrefabPosition.z;

            respawnPos = new Vector3(posX, posY, posZ);
        }

        monsterTransform.position = respawnPos;
    }
}
