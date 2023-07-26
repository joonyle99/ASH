using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAndDestroy : MonoBehaviour
{
    [SerializeField] float _totalDuration = 1f;
    [SerializeField] float _fadeStartTime = 0.5f;

    SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        StartCoroutine(Fade());
    }
    IEnumerator Fade()
    {
        yield return new WaitForSeconds(_fadeStartTime);
        

        float duration = _totalDuration - _fadeStartTime;
        float eTime = 0f;
        Color originalColor = _spriteRenderer.color;
        float a = originalColor.a;
        
        while (eTime < duration)
        {
            eTime += Time.deltaTime;
            yield return null;
            a = Mathf.Lerp(originalColor.a, 0, eTime / duration);
            _spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, a);
        }
        Destroy(gameObject);
    }
}
