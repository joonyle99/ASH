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
        // luminescence�� intensity�� _startIntensity + (-0.5 ~ 0.5) �� ��� �ݺ��ϵ���
        luminescence.intensity = _startIntensity + (Mathf.Sin(Time.time * speed) * 0.5f);
    }
}
