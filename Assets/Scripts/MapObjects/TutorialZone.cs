using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialZone : TriggerZone
{
    [SerializeField] SpriteRenderer _tutorialImage;
    [SerializeField] float _fadeInDuration = 0.3f;
    [SerializeField] float _fadeOffDuration = 0.3f;
    float _originalAlpha = 1f;

    private void Awake()
    {
        _originalAlpha = _tutorialImage.color.a;
        Color color = _tutorialImage.color;
        color.a = 0;
        _tutorialImage.color = color;

    }
    public override void OnPlayerEnter(PlayerBehaviour player)
    {
        StartCoroutine(FadeCoroutine(0, _originalAlpha, _fadeInDuration));
    }
    public override void OnPlayerExit(PlayerBehaviour player)
    {
        StartCoroutine(FadeCoroutine( _originalAlpha, 0, _fadeOffDuration));
    }
    IEnumerator FadeCoroutine(float from, float to, float duration)
    {
        float eTime = 0f;
        Color color = _tutorialImage.color;
        while (eTime < duration)
        {
            color.a = Mathf.Lerp(from, to, eTime / duration);
            _tutorialImage.color = color;
            yield return null;
            eTime += Time.deltaTime;
        }
        color.a = to;
        _tutorialImage.color = color;

    }
}
