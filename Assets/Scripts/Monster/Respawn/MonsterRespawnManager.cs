using System.Collections;
using System.Collections.Generic;
using joonyleTools;
using UnityEngine;

public class MonsterRespawnManager : SingletonBehavior<MonsterRespawnManager>
{
    // 씬에 존재하는 모든 몬스터 리스트
    public List<MonsterBehavior> MonsterList;

    // 몬스터가 사망 후에 리스폰 되기까지의 시간
    public Range RespawnTimeRange;

    protected override void Awake()
    {
        // TODO: 이미 Bootstrap에 자식으로 존재한다
        // base.Awake();

        // 모든 몬스터를 찾고, 리스트에 추가한다
        var monsterArray = FindObjectsOfType<MonsterBehavior>();
        MonsterList = new List<MonsterBehavior>(monsterArray);
    }

    private void RemoveProcess(MonsterBehavior monster)
    {
        // 몬스터 목록에서 제거한다
        //Debug.Log($"removed monster hash code: {monster.GetHashCode()}");
        MonsterList.Remove(monster);

        // 몬스터를 씬에서 삭제한다
        monster.DestroyMonster();
    }
    private void AddProcess(MonsterBehavior.MonsterData monsterData, MonsterBehavior.RespawnData respawnData)
    {
        Debug.Log("Monster Add Process");

        // 프리팹(몬스터를 담은)을 로드한다
        var resourcePath = "Prefabs/Monster/Normal/Prefab_" + monsterData.MonsterName;
        var prefabResource = Resources.Load<GameObject>(resourcePath);

        Debug.Log("인스턴스 생성 바로 직전");

        // 어떤 타입의 몬스터이든, 몬스터를 담은 프리팹의 리스폰 위치는 이전 프리팹의 위치와 동일하다
        // 
        // 새로운 인스턴스가 생성되면 즉시 Awake() 메소드가 호출. 즉 Instantiate 호출이 완료되자마자 바로 실행
        // 해당 게임 오브젝트가 처음 활성화되고 난 '다음 프레임'에서 처음으로 Start() 메소드 실행. 즉, 인스턴스가 생성된 후 처음으로 그 프레임이 시작할 때 호출
        var prefabInstance = Instantiate(prefabResource, respawnData.DefaultPrefabPosition, Quaternion.identity);

        Debug.Log("인스턴스 생성 바로 직후");

        // 새로 생성한 몬스터를 사망 이전의 정보를 토대로 업데이트한다.

        // 행동 반경 정보를 추출한다
        var respawnDataSender = prefabInstance.GetComponentInChildren<RespawnDataSender>();
        // 실질적인 몬스터를 추출한다
        var monsterBehavior = prefabInstance.GetComponentInChildren<MonsterBehavior>();

        // 몬스터의 Material을 변경해줘야 1프레임 동안 이미지가 보이는 버그를 수정할 수 있다.
        monsterBehavior.GetComponent<MaterialController>().InitMaterialForRespawn();

        // 몬스터의 Evaluator를 잠시 대기시켜줘야 리스폰이 완료된 이후에 작동한다.
        var evaluators = monsterBehavior.GetComponents<Evaluator>();
        foreach (var evaluator in evaluators)
        {
            evaluator.StartCoroutine(evaluator.WaitForRespawn());
        }

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
    /// <param name="respawnDataSender"></param>
    /// <param name="moveType"></param>
    /// <param name="respawnData"></param>
    private void UpdateActionAreaInfo(RespawnDataSender respawnDataSender, MonsterDefine.MoveType moveType, MonsterBehavior.RespawnData respawnData)
    {
        Debug.Log("활동 영역 정보 업데이트");

        // 지상 몬스터의 행동 반경 정보 설정
        if (moveType == MonsterDefine.MoveType.GroundNormal)
        {
            // 행동 반경 정보를 가져온다
            respawnDataSender.ExtractActionAreaInfo(out var patrolPointA, out var patrolPointB);

            // * 새로운 행동 반경 정보에 기존의 것을 덮어 씌운다 *

            // 위치 조정
            patrolPointA.transform.position = respawnData.GroundRespawnData.PatrolPointAPosition;
            patrolPointB.transform.position = respawnData.GroundRespawnData.PatrolPointBPosition;

            // transform의 변경사항이 즉시 물리 엔진에 반영되어, bounds가 transform과 동기화된다
            Physics2D.SyncTransforms();

            var groundRespawnDataSender = respawnDataSender as GroundRespawnDataSender;
            if (groundRespawnDataSender == null)
            {
                Debug.LogError("GroundRespawnDataSender가 없습니다");
                return;
            }

            // 변경된 리스폰 정보를 새롭게 생성된 객체에 할당해준다
            groundRespawnDataSender.UpdateRespawnData();
        }
        else if (moveType == MonsterDefine.MoveType.GroundTurret)
        {

        }
        // 공중 몬스터의 행동 반경 정보 설정
        else if (moveType == MonsterDefine.MoveType.Fly)
        {
            // 행동 반경 정보를 가져온다
            respawnDataSender.ExtractActionAreaInfo(out var patrolArea, out var chaseArea);

            // * 새로운 행동 반경 정보에 기존의 것을 덮어 씌운다 *

            // 위치 조정
            patrolArea.transform.position = respawnData.FloatingRespawnData.PatrolAreaPosition;
            chaseArea.transform.position = respawnData.FloatingRespawnData.ChaseAreaPosition;

            // 크기 조정
            patrolArea.transform.localScale = respawnData.FloatingRespawnData.PatrolAreaScale;
            chaseArea.transform.localScale = respawnData.FloatingRespawnData.ChaseAreaScale;

            // transform의 변경사항이 즉시 물리 엔진에 반영되어, bounds가 transform과 동기화된다
            Physics2D.SyncTransforms();

            var floatingRespawnDataSender = respawnDataSender as FloatingRespawnDataSender;
            if (floatingRespawnDataSender == null)
            {
                Debug.LogError("FloatingRespawnDataSender가 없습니다");
                return;
            }

            // 기존의 NavMesh 데이터를 적용한다
            floatingRespawnDataSender.SetNavMeshData(respawnData.FloatingRespawnData.NavMeshData);

            // 변경된 리스폰 정보를 새롭게 생성된 객체에 할당해준다
            floatingRespawnDataSender.UpdateRespawnData();

            // 변경된 행동 반경 정보로 런타임에 NavMesh 굽기
            floatingRespawnDataSender.BakeNavMesh();
        }
    }
    /// <summary>
    /// Prefab_MonsterName에 자식으로 있는 Monster_MonsterName의 리스폰 위치를 설정
    /// </summary>
    /// <param name="monsterTransform"></param>
    /// <param name="moveType"></param>
    /// <param name="respawnData"></param>
    private void UpdateMonsterRespawnPosition(Transform monsterTransform, MonsterDefine.MoveType moveType, MonsterBehavior.RespawnData respawnData)
    {
        Debug.Log("몬스터 리스폰 위치 업데이트");

        var respawnPos = Vector3.zero;

        // 지상 몬스터의 리스폰 위치 설정
        if (moveType == MonsterDefine.MoveType.GroundNormal)
        {
            var t = Random.Range(0.0f, 1.0f); // 0과 1 사이의 임의의 수

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
        // 공중 몬스터의 리스폰 위치 설정
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
            Debug.LogWarning("몬스터 리스트에 제거하려는 몬스터가 존재하지 않습니다");
            return;
        }

        StartCoroutine(RespawnCoroutine(monsterBehavior, RespawnTimeRange.Random()));
    }
    private IEnumerator RespawnCoroutine(MonsterBehavior monsterBehavior, float respawnTime)
    {
        // 몬스터의 데이터를 가져온다
        var monsterData = monsterBehavior.monsterData;
        var respawnData = monsterBehavior.respawnData;

        RemoveProcess(monsterBehavior);

        yield return new WaitForSeconds(respawnTime);

        AddProcess(monsterData, respawnData);
    }
}
