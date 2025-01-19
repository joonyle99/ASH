using TMPro;
using UnityEngine;

public class FPS : MonoBehaviour
{
    public TextMeshProUGUI Text;

    void Update()
    {
        float fps = 1.0f / Time.deltaTime;
        int fpsInt = (int)fps;
        Text.text = $"FPS: {fpsInt}";
    }
}
