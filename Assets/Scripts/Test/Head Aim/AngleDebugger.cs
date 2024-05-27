using UnityEngine;

public class AngleDebugger : MonoBehaviour
{
    public Transform origin;
    public Transform rightTarget;

    private void OnDrawGizmos()
    {
        // Draw line between two objects
        // TODO: Implement the logic to draw the line between two objects

        if(origin && rightTarget)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin.position, rightTarget.position);
        }
    }
}
