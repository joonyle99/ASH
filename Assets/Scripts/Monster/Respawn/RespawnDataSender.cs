using UnityEngine;

public abstract class RespawnDataSender : MonoBehaviour
{
    [Header("Respawn Data Sender")]
    [Space]

    [SerializeField]
    protected MonsterBehavior receiver;

    protected void Start()
    {
        UpdateRespawnData();
    }

    public abstract void ExtractActionAreaInfo(out BoxCollider2D boxCollider1, out BoxCollider2D boxCollider2);
    public abstract void UpdateRespawnData();
}
