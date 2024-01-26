using UnityEngine;
using UnityEngine.UI;

public class TextGlow : MonoBehaviour
{
    private Image[] _images;

    private void Awake()
    {
        _images = GetComponentsInChildren<Image>(true);
    }

    private void Update()
    {
        if (this.gameObject.activeSelf)
        {
            // ���� �ð��� ���� 0���� 1���� ���ϴ� ���İ� ���
            float alpha = (Mathf.Sin(Time.time * Mathf.PI) + 1f) / 2f;

            foreach (Image image in _images)
            {
                var targetColor = image.color;
                targetColor.a = alpha;
                image.color = targetColor;
            }
        }
    }
}
