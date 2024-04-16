using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MonsterRespawnArea : MonoBehaviour
{
    [SerializeField]
    private MonsterBehavior _owner;
    private BoxCollider2D _respawnArea;

    public void Awake()
    {
        _respawnArea = GetComponent<BoxCollider2D>();
    }

    public void Start()
    {
        _owner.SetRespawnBounds(_respawnArea.bounds);
    }
}
