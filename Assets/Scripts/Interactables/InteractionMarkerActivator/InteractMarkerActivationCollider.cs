using UnityEngine;

public class InteractMarkerActivationCollider : MonoBehaviour
{
    InteractableObject _interactableObject;

    void Awake()
    {
        _interactableObject = GetComponentInParent<InteractableObject>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.transform.GetComponent<PlayerBehaviour>();
        if(player != null)
            player.PlayerInteractionController.AddInteractionTarget(_interactableObject);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        var player = collision.transform.GetComponent<PlayerBehaviour>();
        if (player != null)
            player.PlayerInteractionController.RemoveInteractionTarget(_interactableObject);
    }
}