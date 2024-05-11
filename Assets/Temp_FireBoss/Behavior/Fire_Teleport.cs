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
        // 3�ʸ��� �ڷ���Ʈ
        InvokeRepeating("Teleport", 0f, 6f);
    }

    public void Teleport()
    {
        // ���� ���� �ȿ��� �ڷ���Ʈ�Ѵ�
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(0f, teleportRadius);
        Vector2 randomPosition = (Vector2)transform.position + randomDirection * randomDistance;
        transform.position = randomPosition;

        // ��ų ���
        StartCoroutine(fireSkill.StartSkill());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, teleportRadius);
    }
}
