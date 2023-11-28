using UnityEngine;

public class MonsterInstantiator : MonoBehaviour
{
    [SerializeField]
    private GameObject _monstersPrefab; // 몬스터 프리팹

    [SerializeField]
    private Transform[] _monsterLocation; // 몬스터 생성 위치

    // 몬스터 리스트
    // private List<MonsterBehavior> monsters;

    private void Awake()
    {
        // 리스트 생성
        // monsters=  new List<MonsterBehavior>();

        for (int i = 0; i < _monsterLocation.Length; i++)
        {
            // monsterBehavior 게임 오브젝트 생성
            GameObject monsterPrefab = Instantiate(_monstersPrefab, _monsterLocation[i].position, Quaternion.identity);

            // 초기화를 위한 GetComponent<>
            OncologySlime monster = monsterPrefab.GetComponent<OncologySlime>();

            // 몬스터 초기화
            monster.SetUp();

            // monster들의 재생 제어를 위해 리스트에 저장
            // monsters.Add(monsterBehavior);
        }
    }
}
