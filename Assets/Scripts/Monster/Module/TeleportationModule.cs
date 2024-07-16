using UnityEngine;

public class TeleportationModule : MonoBehaviour
{
    public BoxCollider2D teleporyArea;

    public void Teleportation()
    {
        var xPos = Random.Range(teleporyArea.bounds.min.x, teleporyArea.bounds.max.x);
        var yPos = Random.Range(teleporyArea.bounds.min.y, teleporyArea.bounds.max.y);

        var randomPosition = new Vector2(xPos, yPos);

        transform.position = randomPosition;
    }
}
