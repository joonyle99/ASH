using UnityEngine;

public class MonsterBlocking : MonoBehaviour
{
    public MonsterBehavior monster;

    private void Start()
    {
        StartBlocking();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
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
