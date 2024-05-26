using UnityEngine;

public class ScaleViewer : MonoBehaviour
{
    [Header("world scale")]
    [Space]

    public Vector3 worldScale;

    [Header("local scale")]
    [Space]

    public Vector3 localScale;

    // Update is called once per frame
    void Update()
    {
        worldScale = transform.lossyScale;
        localScale = transform.localScale;
    }
}
