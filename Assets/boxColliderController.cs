using UnityEngine;

public class boxColliderController : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private BoxCollider2D _boxCollider;

    void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        Debug.Log($"Initial boxCollider size: {_boxCollider.size}");
    }

    void Update()
    {
        _boxCollider.transform.position = target.position;

        if (Input.GetMouseButtonDown(0))
        {
            _boxCollider.size += Vector2.one;
            Debug.Log($"current boxCollider size: {_boxCollider.size}");
        }
    }
}
