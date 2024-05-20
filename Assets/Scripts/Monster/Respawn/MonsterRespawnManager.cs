using System.Collections;
using System.Collections.Generic;
using joonyleTools;
using UnityEngine;

public class MonsterRespawnManager : SingletonBehavior<MonsterRespawnManager>
{
    // ���� �����ϴ� ��� ���� ����Ʈ
    public List<MonsterBehavior> MonsterList;

    // ���Ͱ� ��� �Ŀ� ������ �Ǳ������ �ð�
    public Range RespawnTimeRange;

    protected override void Awake()
    {
        // TODO: �̹� Bootstrap�� �ڽ����� �����Ѵ�
        // base.Awake();

        // ��� ���͸� ã��, ����Ʈ�� �߰��Ѵ�
        var monsterArray = FindObjectsOfType<MonsterBehavior>();
        MonsterList = new List<MonsterBehavior>(monsterArray);
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
        Debug.Log("Monster Add Process");

        // ������(���͸� ����)�� �ε��Ѵ�
        var resourcePath = "Prefabs/Monster/Normal/Prefab_" + monsterData.MonsterName;
        var prefabResource = Resources.Load<GameObject>(resourcePath);

        Debug.Log("�ν��Ͻ� ���� �ٷ� ����");

        // � Ÿ���� �����̵�, ���͸� ���� �������� ������ ��ġ�� ���� �������� ��ġ�� �����ϴ�
        // 
        // ���ο� �ν��Ͻ��� �����Ǹ� ��� Awake() �޼ҵ尡 ȣ��. �� Instantiate ȣ���� �Ϸ���ڸ��� �ٷ� ����
        // �ش� ���� ������Ʈ�� ó�� Ȱ��ȭ�ǰ� �� '���� ������'���� ó������ Start() �޼ҵ� ����. ��, �ν��Ͻ��� ������ �� ó������ �� �������� ������ �� ȣ��
        var prefabInstance = Instantiate(prefabResource, respawnData.DefaultPrefabPosition, Quaternion.identity);

        Debug.Log("�ν��Ͻ� ���� �ٷ� ����");

        // ���� ������ ���͸� ��� ������ ������ ���� ������Ʈ�Ѵ�.

        // �ൿ �ݰ� ������ �����Ѵ�
        var respawnDataSender = prefabInstance.GetComponentInChildren<RespawnDataSender>();
        // �������� ���͸� �����Ѵ�
        var monsterBehavior = prefabInstance.GetComponentInChildren<MonsterBehavior>();

        // ������ Material�� ��������� 1������ ���� �̹����� ���̴� ���׸� ������ �� �ִ�.
        monsterBehavior.GetComponent<MaterialController>().InitMaterialForRespawn();

        // ������ Evaluator�� ��� ��������� �������� �Ϸ�� ���Ŀ� �۵��Ѵ�.
        var evaluators = monsterBehavior.GetComponents<Evaluator>();
        foreach (var evaluator in evaluators)
        {
            evaluator.StartCoroutine(evaluator.WaitForRespawn());
        }

        // * �ൿ �ݰ� �����͸� �̿��� ������ �ൿ �ݰ� ������ ���� *
        UpdateActionAreaInfo(respawnDataSender, monsterData.MoveType, respawnData);
        // * ������ ��ġ �����͸� �̿��� ������ ��ġ�� ���� *
        UpdateMonsterRespawnPosition(monsterBehavior.transform, monsterData.MoveType, respawnData);

        // ���� ������ ���μ��� ����
        monsterBehavior.RespawnProcess();

        // ���� ��Ͽ� �߰��Ѵ�
        MonsterList.Add(monsterBehavior);
        //Debug.Log($"added monster hash code: {monster.GetHashCode()}");
    }

    /// <summary>
    /// Prefab_MonsterName�� �ൿ �ݰ��� ����
    /// </summary>
    /// <param name="respawnDataSender"></param>
    /// <param name="moveType"></param>
    /// <param name="respawnData"></param>
    private void UpdateActionAreaInfo(RespawnDataSender respawnDataSender, MonsterDefine.MoveType moveType, MonsterBehavior.RespawnData respawnData)
    {
        Debug.Log("Ȱ�� ���� ���� ������Ʈ");

        // ���� ������ �ൿ �ݰ� ���� ����
        if (moveType == MonsterDefine.MoveType.GroundNormal)
        {
            // �ൿ �ݰ� ������ �����´�
            respawnDataSender.ExtractActionAreaInfo(out var patrolPointA, out var patrolPointB);

            // * ���ο� �ൿ �ݰ� ������ ������ ���� ���� ����� *

            // ��ġ ����
            patrolPointA.transform.position = respawnData.GroundRespawnData.PatrolPointAPosition;
            patrolPointB.transform.position = respawnData.GroundRespawnData.PatrolPointBPosition;

            // transform�� ��������� ��� ���� ������ �ݿ��Ǿ�, bounds�� transform�� ����ȭ�ȴ�
            Physics2D.SyncTransforms();

            var groundRespawnDataSender = respawnDataSender as GroundRespawnDataSender;
            if (groundRespawnDataSender == null)
            {
                Debug.LogError("GroundRespawnDataSender�� �����ϴ�");
                return;
            }

            // ����� ������ ������ ���Ӱ� ������ ��ü�� �Ҵ����ش�
            groundRespawnDataSender.UpdateRespawnData();
        }
        else if (moveType == MonsterDefine.MoveType.GroundTurret)
        {

        }
        // ���� ������ �ൿ �ݰ� ���� ����
        else if (moveType == MonsterDefine.MoveType.Fly)
        {
            // �ൿ �ݰ� ������ �����´�
            respawnDataSender.ExtractActionAreaInfo(out var patrolArea, out var chaseArea);

            // * ���ο� �ൿ �ݰ� ������ ������ ���� ���� ����� *

            // ��ġ ����
            patrolArea.transform.position = respawnData.FloatingRespawnData.PatrolAreaPosition;
            chaseArea.transform.position = respawnData.FloatingRespawnData.ChaseAreaPosition;

            // ũ�� ����
            patrolArea.transform.localScale = respawnData.FloatingRespawnData.PatrolAreaScale;
            chaseArea.transform.localScale = respawnData.FloatingRespawnData.ChaseAreaScale;

            // transform�� ��������� ��� ���� ������ �ݿ��Ǿ�, bounds�� transform�� ����ȭ�ȴ�
            Physics2D.SyncTransforms();

            var floatingRespawnDataSender = respawnDataSender as FloatingRespawnDataSender;
            if (floatingRespawnDataSender == null)
            {
                Debug.LogError("FloatingRespawnDataSender�� �����ϴ�");
                return;
            }

            // ������ NavMesh �����͸� �����Ѵ�
            floatingRespawnDataSender.SetNavMeshData(respawnData.FloatingRespawnData.NavMeshData);

            // ����� ������ ������ ���Ӱ� ������ ��ü�� �Ҵ����ش�
            floatingRespawnDataSender.UpdateRespawnData();

            // ����� �ൿ �ݰ� ������ ��Ÿ�ӿ� NavMesh ����
            floatingRespawnDataSender.BakeNavMesh();
        }
    }
    /// <summary>
    /// Prefab_MonsterName�� �ڽ����� �ִ� Monster_MonsterName�� ������ ��ġ�� ����
    /// </summary>
    /// <param name="monsterTransform"></param>
    /// <param name="moveType"></param>
    /// <param name="respawnData"></param>
    private void UpdateMonsterRespawnPosition(Transform monsterTransform, MonsterDefine.MoveType moveType, MonsterBehavior.RespawnData respawnData)
    {
        Debug.Log("���� ������ ��ġ ������Ʈ");

        var respawnPos = Vector3.zero;

        // ���� ������ ������ ��ġ ����
        if (moveType == MonsterDefine.MoveType.GroundNormal)
        {
            var t = Random.Range(0.0f, 1.0f); // 0�� 1 ������ ������ ��

            var posX = Mathf.Lerp(respawnData.GroundRespawnData.RespawnLine.pointA.x, respawnData.GroundRespawnData.RespawnLine.pointB.x, t);
            var posY = Mathf.Lerp(respawnData.GroundRespawnData.RespawnLine.pointA.y, respawnData.GroundRespawnData.RespawnLine.pointB.y, t);
            var posZ = respawnData.DefaultPrefabPosition.z;

            respawnPos = new Vector3(posX, posY, posZ);

            // Debug.DrawRay(respawnPos, Vector3.up, Color.red, 3f);
        }
        else if (moveType == MonsterDefine.MoveType.GroundTurret)
        {
            respawnPos = respawnData.DefaultPrefabPosition;
        }
        // ���� ������ ������ ��ġ ����
        else if (moveType == MonsterDefine.MoveType.Fly)
        {
            var respawnBounds = respawnData.FloatingRespawnData.RespawnBounds;

            var posX = Random.Range(respawnBounds.min.x, respawnBounds.max.x);
            var posY = Random.Range(respawnBounds.min.y, respawnBounds.max.y);
            var posZ = respawnData.DefaultPrefabPosition.z;

            respawnPos = new Vector3(posX, posY, posZ);

            monsterTransform.GetComponent<FloatingMovementModule>().SetPosition(respawnPos);
            return;
        }

        monsterTransform.position = respawnPos;
    }

    public void NotifyDeath(MonsterBehavior monsterBehavior)
    {
        if (!MonsterList.Contains(monsterBehavior))
        {
            Debug.LogWarning("���� ����Ʈ�� �����Ϸ��� ���Ͱ� �������� �ʽ��ϴ�");
            return;
        }

        StartCoroutine(RespawnCoroutine(monsterBehavior, RespawnTimeRange.Random()));
    }
    private IEnumerator RespawnCoroutine(MonsterBehavior monsterBehavior, float respawnTime)
    {
        // ������ �����͸� �����´�
        var monsterData = monsterBehavior.monsterData;
        var respawnData = monsterBehavior.respawnData;

        RemoveProcess(monsterBehavior);

        yield return new WaitForSeconds(respawnTime);

        AddProcess(monsterData, respawnData);
    }
}
