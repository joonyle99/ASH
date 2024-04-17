using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider2D))]
public class MonsterPatrolArea : MonoBehaviour
{
    [SerializeField]
    private MonsterBehavior _monster;
    private BoxCollider2D _patrolArea;

    public void Awake()
    {
        _patrolArea = GetComponent<BoxCollider2D>();
    }

    public void Start()
    {
        _monster.SetPatrolArea(_patrolArea.bounds);
    }

    private void OnDrawGizmos()
    {
        // Patrol Area의 영역을 표시한다
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_patrolArea.bounds.center, _patrolArea.bounds.size);
    }
}
