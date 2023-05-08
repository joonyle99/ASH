using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterInstantiator : MonoBehaviour
{
    [SerializeField]
    private GameObject monstersPrefab; // 몬스터 프리팹

    [SerializeField]
    private Transform[] monsterLocation; // 몬스터 생성 위치

    [SerializeField]
    private string[] monseterNames; // 몬스터 이름

    // 몬스터 리스트
    // private List<BasedMonster> monsters;

    private void Awake()
    {
        // 리스트 생성
        // monsters=  new List<BasedMonster>();

        for (int i = 0; i < monsterLocation.Length; i++)
        {
            // monster 게임 오브젝트 생성
            GameObject monsterPrefab = Instantiate(monstersPrefab, monsterLocation[i].position, Quaternion.identity);

            // 초기화를 위한 GetComponent<>
            OncologySlime monster = monsterPrefab.GetComponent<OncologySlime>();

            // 몬스터 초기화
            monster.SetUp(monseterNames[i], 100 * (i + 1));

            // monster들의 재생 제어를 위해 리스트에 저장
            // monsters.Add(monster);
        }
    }
}
