using UnityEngine;

public class MonsterBlocking : MonoBehaviour
{
    public MonsterBehaviour monster;

    private void Start()
    {
        StartBlocking();
    }

    private void Update()
    {
        // CHEAT: F6 Ű�� ������ ���� ���ŷ ����
        if (Input.GetKeyDown(KeyCode.F6) && GameSceneManager.Instance.CheatMode == true)
        {
            EndBlocking();
        }
    }

    public void StartBlocking()
    {
        monster.enabled = false;
        monster.SetAnimatorTrigger("StartBlocking");
    }

    public void EndBlocking()
    {
        monster.enabled = true;
        monster.SetAnimatorTrigger("EndBlocking");
    }

    public void EndBlocking_OnlyEnable()
    {
        monster.enabled = true;
    }
}
