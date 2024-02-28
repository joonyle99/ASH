using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialZone : TriggerZone
{
    [SerializeField] GameObject skillObject;
    [SerializeField] float _fadeInDuration = 0.3f;
    [SerializeField] float _fadeOffDuration = 0.3f;

    private float _originalAlpha = 1f;

    public Image[] images;
    public TextMeshProUGUI[] textMeshs;

    private void Awake()
    {
        images = skillObject.GetComponentsInChildren<Image>();
        textMeshs = skillObject.GetComponentsInChildren<TextMeshProUGUI>();

        // √ ±‚»≠
        foreach (var image in images)
        {
            Color tempColor = image.color;
            tempColor.a = 0f;
            image.color = tempColor;
        }
        foreach (var textMesh in textMeshs)
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
            Color[] tempColors = new Color[images.Length + textMeshs.Length];

            for (int i = 0; i < images.Length; i++)
            {
                float alpha = Mathf.Lerp(from, to, eTime / duration);
                tempColors[i] = images[i].color;
                tempColors[i].a = alpha;
                images[i].color = tempColors[i];
            }
            for (int i = 0; i < textMeshs.Length; i++)
            {
                float alpha = Mathf.Lerp(from, to, eTime / duration);
                tempColors[images.Length + i] = textMeshs[i].color;
                tempColors[images.Length + i].a = alpha;
                textMeshs[i].color = tempColors[images.Length + i];
            }

            eTime += Time.deltaTime;
            yield return null;
        }
    }
}
