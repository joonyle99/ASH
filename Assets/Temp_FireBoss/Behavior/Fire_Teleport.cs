using UnityEngine;

public class Fire_Teleport : MonoBehaviour
{
    public float teleportRadius = 10f;
    public Fire_Skill fireSkill;

    public void Awake()
    {
        fireSkill = GetComponent<Fire_Skill>();
    }
    public void Start()
    {
        // 3초마다 텔레포트
        InvokeRepeating("Teleport", 0f, 6f);
    }

    public void Teleport()
    {
        // 일정 범위 안에서 텔레포트한다
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(0f, teleportRadius);
        Vector2 randomPosition = (Vector2)transform.position + randomDirection * randomDistance;
        transform.position = randomPosition;

        // 스킬 사용
        StartCoroutine(fireSkill.StartSkill());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, teleportRadius);
    }
}
