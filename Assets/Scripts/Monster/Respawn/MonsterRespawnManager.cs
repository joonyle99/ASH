using System.Collections;
using System.Collections.Generic;
using joonyle99;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterRespawnManager : HappyTools.SingletonBehaviourFixed<MonsterRespawnManager>
{
    // 씬에 존재하는 모든 몬스터 리스트
    public List<MonsterBehaviour> MonsterList;

    // 몬스터가 사망 후에 리스폰 되기까지의 시간
    public Range RespawnTimeRange;

    protected override void Awake()
    {
        base.Awake();

        SceneManager.sceneLoaded += OnSceneLoaded;

        // 모든 몬스터를 찾고, 리스트에 추가한다
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
        // 데이터를 가져온다
        var monsterData = monsterBehavior.monsterData;
        var respawnData = monsterBehavior.respawnData;

        RemoveProcess(monsterBehavior);

        yield return new WaitForSeconds(respawnTime);

        AddProcess(monsterData, respawnData);
    }

    private void RemoveProcess(MonsterBehaviour monster)
    {
        //Debug.Log($"removed monster hash code: {monster.GetHashCode()}");

        // 몬스터를 리스트에서 제거한다
        MonsterList.Remove(monster);

        // 몬스터를 씬에서 삭제한다
        monster.DestroyMonsterPrefab();
    }
    private void AddProcess(MonsterBehaviour.MonsterData monsterData, MonsterBehaviour.RespawnData respawnData)
    {
        // Debug.Log("Monster Add Process");

        // 프리팹(몬스터를 담은)을 로드한다
        var resourcePath = "Prefabs/Monster/Normal/Prefab_" + monsterData.MonsterName;
        var prefabResource = Resources.Load<GameObject>(resourcePath);

        // Debug.Log("인스턴스 생성 바로 직전");

        // 
        // 설명:
        //      어떤 타입의 몬스터이든, 몬스터를 담은 프리팹의 리스폰 위치는 이전 프리팹의 위치와 동일하다
        // 
        // 추가:
        //      1. Instantiate(): 새로운 게임 오브젝트가 생성되고 메모리에 할당된다.
        //      2. Awake(): 오브젝트가 메모리에 할당되자마자 즉시 호출된다. (컴포넌트나 스크립트 초기화 작업에 적합)
        //      3. OnEnable(): 오브젝트가 활성화 상태로 전환될 때 호출된다.
        //      4. MemberFunction(): 첫번째 초기화 작업인 Awake()와 OnEnable() 이후에, 그리고 Start()가 호출되기 전 호출된다.
        //      5. Start(): Awake()와 OnEnable() 이후, 첫번째 프레임이 Update() 되기전에 호출된다.
        var prefabInstance = Instantiate(prefabResource, respawnData.DefaultPrefabPosition, Quaternion.identity);

        // Debug.Log("인스턴스 생성 바로 직후");

        // 새로 생성한 몬스터를 사망 이전의 정보를 토대로 업데이트한다.

        // 행동 반경 정보를 추출한다
        var respawnDataSender = prefabInstance.GetComponentInChildren<ActionAreaDataSender>();
        // 프리팹의 자식에 있는 실질적인 몬스터를 추출한다
        var monsterBehavior = prefabInstance.GetComponentInChildren<MonsterBehaviour>();

        // TEMP: 임시 함수
        InstantFunction(monsterBehavior);

        // * 행동 반경 데이터를 이용해 몬스터의 행동 반경 정보를 변경 *
        UpdateActionAreaInfo(respawnDataSender, monsterData.MoveType, respawnData);
        // * 리스폰 위치 데이터를 이용해 몬스터의 위치를 변경 *
        UpdateMonsterRespawnPosition(monsterBehavior.transform, monsterData.MoveType, respawnData);

        // 몬스터 리스폰 프로세스 실행
        monsterBehavior.RespawnProcess();

        // 몬스터 목록에 추가한다
        MonsterList.Add(monsterBehavior);

        //Debug.Log($"added monster hash code: {monster.GetHashCode()}");
    }

    /// <summary>
    /// Prefab_MonsterName의 행동 반경을 설정
    /// </summary>
    /// <param name="actionAreaDataSender"></param>
    /// <param name="moveType"></param>
    /// <param name="respawnData"></param>
    private void UpdateActionAreaInfo(ActionAreaDataSender actionAreaDataSender, MonsterDefine.MoveType moveType, MonsterBehaviour.RespawnData respawnData)
    {
        Debug.Log("리스폰된 몬스터의 활동 영역 정보를 업데이트");

        // 지상 몬스터의 행동 반경 정보 설정
        if (moveType == MonsterDefine.MoveType.GroundNormal)
        {
            // 행동 반경 정보를 가져온다
            actionAreaDataSender.ExtractActionAreaInfo(out var patrolPointA, out var patrolPointB);

            // * 새로운 행동 반경 정보에 기존의 것을 덮어 씌운다 *

            // 위치 조정
            patrolPointA.transform.position = respawnData.groundActionAreaData.PatrolPointAPosition;
            patrolPointB.transform.position = respawnData.groundActionAreaData.PatrolPointBPosition;

            // transform의 변경사항이 즉시 물리 엔진에 반영되어, bounds가 transform과 동기화된다
            UnityEngine.Physics2D.SyncTransforms();

            // 변경된 리스폰 정보를 새롭게 생성된 객체에 할당해준다
            actionAreaDataSender.UpdateActionAreaData();
        }
        // 공중 몬스터의 행동 반경 정보 설정
        else if (moveType == MonsterDefine.MoveType.Fly)
        {
            // 행동 반경 정보를 가져온다
            actionAreaDataSender.ExtractActionAreaInfo(out var patrolArea, out var chaseArea);

            // * 새로운 행동 반경 정보에 기존의 것을 덮어 씌운다 *

            // 위치 조정
            patrolArea.transform.position = respawnData.floatingActionAreaData.PatrolAreaPosition;
            chaseArea.transform.position = respawnData.floatingActionAreaData.ChaseAreaPosition;

            // 크기 조정
            patrolArea.transform.localScale = respawnData.floatingActionAreaData.PatrolAreaScale;
            chaseArea.transform.localScale = respawnData.floatingActionAreaData.ChaseAreaScale;

            // transform의 변경사항이 즉시 물리 엔진에 반영되어, bounds가 transform과 동기화된다
            UnityEngine.Physics2D.SyncTransforms();

            // 변경된 리스폰 정보를 새롭게 생성된 객체에 할당해준다
            actionAreaDataSender.UpdateActionAreaData();
        }
    }
    /// <summary>
    /// Prefab_MonsterName에 자식으로 있는 Monster_MonsterName의 리스폰 위치를 설정
    /// </summary>
    /// <param name="monsterTransform"></param>
    /// <param name="moveType"></param>
    /// <param name="respawnData"></param>
    private void UpdateMonsterRespawnPosition(Transform monsterTransform, MonsterDefine.MoveType moveType, MonsterBehaviour.RespawnData respawnData)
    {
        Debug.Log("리스폰된 몬스터의 리스폰 위치 업데이트");

        var respawnPos = Vector3.zero;

        // 지상 몬스터의 리스폰 위치 설정
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
        // 공중 몬스터의 리스폰 위치 설정
        else if (moveType == MonsterDefine.MoveType.Fly)
        {
            var respawnBounds = respawnData.floatingActionAreaData.RespawnBounds;

            var posX = Random.Range(respawnBounds.min.x, respawnBounds.max.x);
            var posY = Random.Range(respawnBounds.min.y, respawnBounds.max.y);
            var posZ = respawnData.DefaultPrefabPosition.z;

            respawnPos = new Vector3(posX, posY, posZ);

            // floating monster는 warp를 사용한다
            monsterTransform.GetComponent<FloatingMovementModule>().SetPosition(respawnPos);
            return;
        }

        monsterTransform.position = respawnPos;
    }

    private void InstantFunction(MonsterBehaviour monsterBehavior)
    {
        // TEMP: 몬스터의 Material을 변경해줘야 1프레임 동안 이미지가 보이는 버그를 수정할 수 있다.
        monsterBehavior.GetComponent<MaterialController>().InitMaterialForRespawn();

        // TEMP: 몬스터의 Evaluator를 잠시 대기시켜줘야 리스폰이 완료된 이후에 작동한다.
        var evaluators = monsterBehavior.GetComponents<Evaluator>();
        foreach (var evaluator in evaluators)
        {
            evaluator.StartCoroutine(evaluator.WaitForRespawn());
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // MonsterList를 비운다
        MonsterList.Clear();

        // 모든 몬스터를 찾고, 리스트에 추가한다
        var monsterArray = FindObjectsOfType<MonsterBehaviour>();
        MonsterList = new List<MonsterBehaviour>(monsterArray);
    }
}
