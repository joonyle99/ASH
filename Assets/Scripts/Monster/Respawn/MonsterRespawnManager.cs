using System.Collections;
using System.Collections.Generic;
using joonyle99;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterRespawnManager : HappyTools.SingletonBehaviourFixed<MonsterRespawnManager>
{
    // ���� �����ϴ� ��� ���� ����Ʈ
    public List<MonsterBehaviour> MonsterList;

    // ���Ͱ� ��� �Ŀ� ������ �Ǳ������ �ð�
    public Range RespawnTimeRange;

    protected override void Awake()
    {
        base.Awake();

        SceneManager.sceneLoaded += OnSceneLoaded;

        // ��� ���͸� ã��, ����Ʈ�� �߰��Ѵ�
        var monsterArray = FindObjectsOfType<MonsterBehaviour>();
        MonsterList = new List<MonsterBehaviour>(monsterArray);
    }

    public void NotifyDeath(MonsterBehaviour monsterBehavior)
    {
        if (!MonsterList.Contains(monsterBehavior))
        {
            Debug.LogWarning("This monster doesn't exist in the list");
            return;
        }

        var respawnTime = RespawnTimeRange.Random();
        StartCoroutine(RespawnCoroutine(monsterBehavior, respawnTime));
    }
    private IEnumerator RespawnCoroutine(MonsterBehaviour monsterBehavior, float respawnTime)
    {
        // �����͸� �����´�
        var monsterData = monsterBehavior.monsterData;
        var respawnData = monsterBehavior.respawnData;

        RemoveProcess(monsterBehavior);

        yield return new WaitForSeconds(respawnTime);

        AddProcess(monsterData, respawnData);
    }

    private void RemoveProcess(MonsterBehaviour monster)
    {
        //Debug.Log($"removed monster hash code: {monster.GetHashCode()}");

        // ���͸� ����Ʈ���� �����Ѵ�
        MonsterList.Remove(monster);

        // ���͸� ������ �����Ѵ�
        monster.DestroyMonsterPrefab();
    }
    private void AddProcess(MonsterBehaviour.MonsterData monsterData, MonsterBehaviour.RespawnData respawnData)
    {
        // Debug.Log("Monster Add Process");

        // ������(���͸� ����)�� �ε��Ѵ�
        var resourcePath = "Prefabs/Monster/Normal/Prefab_" + monsterData.MonsterName;
        var prefabResource = Resources.Load<GameObject>(resourcePath);

        // Debug.Log("�ν��Ͻ� ���� �ٷ� ����");

        // 
        // ����:
        //      � Ÿ���� �����̵�, ���͸� ���� �������� ������ ��ġ�� ���� �������� ��ġ�� �����ϴ�
        // 
        // �߰�:
        //      1. Instantiate(): ���ο� ���� ������Ʈ�� �����ǰ� �޸𸮿� �Ҵ�ȴ�.
        //      2. Awake(): ������Ʈ�� �޸𸮿� �Ҵ���ڸ��� ��� ȣ��ȴ�. (������Ʈ�� ��ũ��Ʈ �ʱ�ȭ �۾��� ����)
        //      3. OnEnable(): ������Ʈ�� Ȱ��ȭ ���·� ��ȯ�� �� ȣ��ȴ�.
        //      4. MemberFunction(): ù��° �ʱ�ȭ �۾��� Awake()�� OnEnable() ���Ŀ�, �׸��� Start()�� ȣ��Ǳ� �� ȣ��ȴ�.
        //      5. Start(): Awake()�� OnEnable() ����, ù��° �������� Update() �Ǳ����� ȣ��ȴ�.
        var prefabInstance = Instantiate(prefabResource, respawnData.DefaultPrefabPosition, Quaternion.identity);

        // Debug.Log("�ν��Ͻ� ���� �ٷ� ����");

        // ���� ������ ���͸� ��� ������ ������ ���� ������Ʈ�Ѵ�.

        // �ൿ �ݰ� ������ �����Ѵ�
        var respawnDataSender = prefabInstance.GetComponentInChildren<ActionAreaDataSender>();
        // �������� �ڽĿ� �ִ� �������� ���͸� �����Ѵ�
        var monsterBehavior = prefabInstance.GetComponentInChildren<MonsterBehaviour>();

        // TEMP: �ӽ� �Լ�
        InstantFunction(monsterBehavior);

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
    /// <param name="actionAreaDataSender"></param>
    /// <param name="moveType"></param>
    /// <param name="respawnData"></param>
    private void UpdateActionAreaInfo(ActionAreaDataSender actionAreaDataSender, MonsterDefine.MoveType moveType, MonsterBehaviour.RespawnData respawnData)
    {
        Debug.Log("�������� ������ Ȱ�� ���� ������ ������Ʈ");

        // ���� ������ �ൿ �ݰ� ���� ����
        if (moveType == MonsterDefine.MoveType.GroundNormal)
        {
            // �ൿ �ݰ� ������ �����´�
            actionAreaDataSender.ExtractActionAreaInfo(out var patrolPointA, out var patrolPointB);

            // * ���ο� �ൿ �ݰ� ������ ������ ���� ���� ����� *

            // ��ġ ����
            patrolPointA.transform.position = respawnData.groundActionAreaData.PatrolPointAPosition;
            patrolPointB.transform.position = respawnData.groundActionAreaData.PatrolPointBPosition;

            // transform�� ��������� ��� ���� ������ �ݿ��Ǿ�, bounds�� transform�� ����ȭ�ȴ�
            UnityEngine.Physics2D.SyncTransforms();

            // ����� ������ ������ ���Ӱ� ������ ��ü�� �Ҵ����ش�
            actionAreaDataSender.UpdateActionAreaData();
        }
        // ���� ������ �ൿ �ݰ� ���� ����
        else if (moveType == MonsterDefine.MoveType.Fly)
        {
            // �ൿ �ݰ� ������ �����´�
            actionAreaDataSender.ExtractActionAreaInfo(out var patrolArea, out var chaseArea);

            // * ���ο� �ൿ �ݰ� ������ ������ ���� ���� ����� *

            // ��ġ ����
            patrolArea.transform.position = respawnData.floatingActionAreaData.PatrolAreaPosition;
            chaseArea.transform.position = respawnData.floatingActionAreaData.ChaseAreaPosition;

            // ũ�� ����
            patrolArea.transform.localScale = respawnData.floatingActionAreaData.PatrolAreaScale;
            chaseArea.transform.localScale = respawnData.floatingActionAreaData.ChaseAreaScale;

            // transform�� ��������� ��� ���� ������ �ݿ��Ǿ�, bounds�� transform�� ����ȭ�ȴ�
            UnityEngine.Physics2D.SyncTransforms();

            // ����� ������ ������ ���Ӱ� ������ ��ü�� �Ҵ����ش�
            actionAreaDataSender.UpdateActionAreaData();
        }
    }
    /// <summary>
    /// Prefab_MonsterName�� �ڽ����� �ִ� Monster_MonsterName�� ������ ��ġ�� ����
    /// </summary>
    /// <param name="monsterTransform"></param>
    /// <param name="moveType"></param>
    /// <param name="respawnData"></param>
    private void UpdateMonsterRespawnPosition(Transform monsterTransform, MonsterDefine.MoveType moveType, MonsterBehaviour.RespawnData respawnData)
    {
        Debug.Log("�������� ������ ������ ��ġ ������Ʈ");

        var respawnPos = Vector3.zero;

        // ���� ������ ������ ��ġ ����
        if (moveType == MonsterDefine.MoveType.GroundNormal)
        {
            var t = Random.Range(0f, 1f);

            var pointAx = respawnData.groundActionAreaData.respawnLine3D.pointA.x;
            var pointBx = respawnData.groundActionAreaData.respawnLine3D.pointB.x;

            var pointAy = respawnData.groundActionAreaData.respawnLine3D.pointA.y;
            var pointBy = respawnData.groundActionAreaData.respawnLine3D.pointB.y;

            var pointA = new Vector2(pointAx, pointAy);
            var pointB = new Vector2(pointBx, pointBy);

            var respawnLine = new Line2D(pointA, pointB);

            var respawnPoint = Line2D.Lerp(respawnLine, t);

            var posX = respawnPoint.x;
            var posY = respawnPoint.y;
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
            var respawnBounds = respawnData.floatingActionAreaData.RespawnBounds;

            var posX = Random.Range(respawnBounds.min.x, respawnBounds.max.x);
            var posY = Random.Range(respawnBounds.min.y, respawnBounds.max.y);
            var posZ = respawnData.DefaultPrefabPosition.z;

            respawnPos = new Vector3(posX, posY, posZ);

            // floating monster�� warp�� ����Ѵ�
            monsterTransform.GetComponent<FloatingMovementModule>().SetPosition(respawnPos);
            return;
        }

        monsterTransform.position = respawnPos;
    }

    private void InstantFunction(MonsterBehaviour monsterBehavior)
    {
        // TEMP: ������ Material�� ��������� 1������ ���� �̹����� ���̴� ���׸� ������ �� �ִ�.
        monsterBehavior.GetComponent<MaterialController>().InitMaterialForRespawn();

        // TEMP: ������ Evaluator�� ��� ��������� �������� �Ϸ�� ���Ŀ� �۵��Ѵ�.
        var evaluators = monsterBehavior.GetComponents<Evaluator>();
        foreach (var evaluator in evaluators)
        {
            evaluator.StartCoroutine(evaluator.WaitForRespawn());
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // MonsterList�� ����
        MonsterList.Clear();

        // ��� ���͸� ã��, ����Ʈ�� �߰��Ѵ�
        var monsterArray = FindObjectsOfType<MonsterBehaviour>();
        MonsterList = new List<MonsterBehaviour>(monsterArray);
    }
}
