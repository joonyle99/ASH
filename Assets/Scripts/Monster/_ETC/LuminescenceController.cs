using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LuminescenceController : MonoBehaviour
{
    public Light2D luminescence;
    public float speed;

    private float _startIntensity;

    void Awake()
    {
        luminescence = GetComponent<Light2D>();
        _startIntensity = luminescence.intensity;
    }

    void Update()
    {
        // luminescence의 intensity를 _startIntensity + (-0.5 ~ 0.5) 를 계속 반복하도록
        luminescence.intensity = _startIntensity + (Mathf.Sin(Time.time * speed) * 0.5f);
    }
}
