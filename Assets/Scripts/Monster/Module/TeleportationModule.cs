using UnityEngine;

public class TeleportationModule : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _teleportableArea;

    public void ExecuteTeleportation_AnimEvent()
    {
        var xPos = Random.Range(_teleportableArea.bounds.min.x, _teleportableArea.bounds.max.x);
        var yPos = Random.Range(_teleportableArea.bounds.min.y, _teleportableArea.bounds.max.y);

        var randomPosition = new Vector2(xPos, yPos);

        transform.position = randomPosition;
    }
}
