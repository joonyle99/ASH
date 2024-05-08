using UnityEngine;

public class Fire_Teleport : MonoBehaviour
{
    private Fire_Skill fire_skill;

    public float teleportRadius = 10f;

    public void Awake()
    {
        fire_skill = GetComponent<Fire_Skill>();
    }
    public void Start()
    {
        InvokeRepeating("Teleport", 0f, 3f);
    }

    public void Teleport()
    {
        // 일정 범위 안에서 텔레포트한다
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Debug.Log($"{randomDirection} / {randomDirection.magnitude}");
        float randomDistance = Random.Range(0f, teleportRadius);
        Vector2 randomPosition = (Vector2)transform.position + randomDirection * randomDistance;
        transform.position = randomPosition;

        StartCoroutine(fire_skill.StartSkill());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, teleportRadius);
    }


}
