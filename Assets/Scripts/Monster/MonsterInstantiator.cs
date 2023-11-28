using UnityEngine;

public class MonsterInstantiator : MonoBehaviour
{
    [SerializeField]
    private GameObject _monstersPrefab; // ���� ������

    [SerializeField]
    private Transform[] _monsterLocation; // ���� ���� ��ġ

    // ���� ����Ʈ
    // private List<MonsterBehavior> monsters;

    private void Awake()
    {
        // ����Ʈ ����
        // monsters=  new List<MonsterBehavior>();

        for (int i = 0; i < _monsterLocation.Length; i++)
        {
            // monsterBehavior ���� ������Ʈ ����
            GameObject monsterPrefab = Instantiate(_monstersPrefab, _monsterLocation[i].position, Quaternion.identity);

            // �ʱ�ȭ�� ���� GetComponent<>
            OncologySlime monster = monsterPrefab.GetComponent<OncologySlime>();

            // ���� �ʱ�ȭ
            monster.SetUp();

            // monster���� ��� ��� ���� ����Ʈ�� ����
            // monsters.Add(monsterBehavior);
        }
    }
}
