using UnityEngine;

public class RotationTest : MonoBehaviour
{
    public Vector3 localRotation;
    public Vector3 worldRotation;

    void Update()
    {
        localRotation = transform.localEulerAngles;
        worldRotation = transform.eulerAngles;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, this.transform.position + transform.up * 5f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(this.transform.position, this.transform.position + transform.right * 5f);
    }

}
