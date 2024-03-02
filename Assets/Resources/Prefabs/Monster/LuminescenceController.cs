using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LuminescenceController : MonoBehaviour
{
    public Light2D luminescence;
    public float speed;

    void Awake()
    {
        luminescence = GetComponent<Light2D>();
    }

    void Update()
    {
        // luminescence의 intensity를 0.3 ~ 0.8을 계속 반복하도록
        luminescence.intensity = (Mathf.Sin(Time.time * speed) * 0.25f) + 0.55f;
    }
}
