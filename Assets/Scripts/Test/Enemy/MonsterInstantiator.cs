using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterInstantiator : MonoBehaviour
{
    [SerializeField]
    private GameObject monstersPrefab; // ���� ������

    [SerializeField]
    private Transform[] monsterLocation; // ���� ���� ��ġ

    [SerializeField]
    private string[] monseterNames; // ���� �̸�

    // ���� ����Ʈ
    // private List<BasedMonster> monsters;

    private void Awake()
    {
        // ����Ʈ ����
        // monsters=  new List<BasedMonster>();

        for (int i = 0; i < monsterLocation.Length; i++)
        {
            // monster ���� ������Ʈ ����
            GameObject monsterPrefab = Instantiate(monstersPrefab, monsterLocation[i].position, Quaternion.identity);

            // �ʱ�ȭ�� ���� GetComponent<>
            OncologySlime monster = monsterPrefab.GetComponent<OncologySlime>();

            // ���� �ʱ�ȭ
            monster.SetUp(monseterNames[i], 100 * (i + 1));

            // monster���� ��� ��� ���� ����Ʈ�� ����
            // monsters.Add(monster);
        }
    }
}
