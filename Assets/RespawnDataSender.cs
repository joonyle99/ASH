using UnityEngine;

public abstract class RespawnDataSender : MonoBehaviour
{
    [Header("RespawnDataSender")]
    [Space]

    [SerializeField]
    protected MonsterBehavior receiver;

    public abstract void ExtractActionAreaInfo(out BoxCollider2D boxCollider1, out BoxCollider2D boxCollider2);
    public abstract void UpdateRespawnData();
}
