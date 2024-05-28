using UnityEngine;

public class HeadAimTargetDebugger : MonoBehaviour
{
    public Transform origin;
    public Transform rightTarget;

    private void OnDrawGizmos()
    {
        if (!origin) return;

        Gizmos.color = Color.red;

        if (rightTarget)
        {
            Gizmos.DrawLine(origin.position, rightTarget.position);
        }
    }
}
