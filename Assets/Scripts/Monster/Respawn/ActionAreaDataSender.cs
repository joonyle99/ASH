using UnityEngine;

public abstract class ActionAreaDataSender : MonoBehaviour
{
    [Header("ActionArea Data Sender")]
    [Space]

    [SerializeField]
    protected MonsterBehavior receiver;

    protected void Start()
    {
        UpdateActionAreaData();
    }

    public abstract void UpdateActionAreaData();
    public abstract void ExtractActionAreaInfo(out BoxCollider2D boxCollider1, out BoxCollider2D boxCollider2);
}
