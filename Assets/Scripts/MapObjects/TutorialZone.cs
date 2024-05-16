using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialZone : TriggerZone
{
    [SerializeField] private GameObject skillObject;
    [SerializeField] private float _fadeInDuration = 0.3f;
    [SerializeField] private float _fadeOffDuration = 0.3f;

    private float _originalAlpha = 1f;

    private Image[] _images;
    private TextMeshProUGUI[] _texts;

    private void Awake()
    {
        // get every image and text
        _images = skillObject.GetComponentsInChildren<Image>();
        _texts = skillObject.GetComponentsInChildren<TextMeshProUGUI>();

        foreach (var image in _images)
        {
            Color tempColor = image.color;
            tempColor.a = 0f;
            image.color = tempColor;
        }
        foreach (var textMesh in _texts)
        {
            Color tempColor = textMesh.color;
            tempColor.a = 0f;
            textMesh.color = tempColor;
        }
    }

    public override void OnPlayerEnter(PlayerBehaviour player)
    {
        StartCoroutine(FadeAllCoroutine(0f, _originalAlpha, _fadeInDuration));
    }
    public override void OnPlayerExit(PlayerBehaviour player)
    {
        StartCoroutine(FadeAllCoroutine(_originalAlpha, 0f, _fadeOffDuration));
    }
    IEnumerator FadeAllCoroutine(float from, float to, float duration)
    {
        float eTime = 0f;
        while (eTime < duration)
        {
            Color[] tempColors = new Color[_images.Length + _texts.Length];

            for (int i = 0; i < _images.Length; i++)
            {
                float alpha = Mathf.Lerp(from, to, eTime / duration);
                tempColors[i] = _images[i].color;
                tempColors[i].a = alpha;
                _images[i].color = tempColors[i];
            }
            for (int i = 0; i < _texts.Length; i++)
            {
                float alpha = Mathf.Lerp(from, to, eTime / duration);
                tempColors[_images.Length + i] = _texts[i].color;
                tempColors[_images.Length + i].a = alpha;
                _texts[i].color = tempColors[_images.Length + i];
            }

            eTime += Time.deltaTime;
            yield return null;
        }
    }
}
