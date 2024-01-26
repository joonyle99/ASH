using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KeyMapManager : MonoBehaviour
{
    [SerializeField] KeyCode _showKey;

    [Space]

    [SerializeField] private GameObject _keyMap;
    [SerializeField] private float _firstDelay;

    [Space]

    [SerializeField] private float _targetFadeTime;
    [SerializeField] private float _elapsedFadeTime;

    [Space]

    [SerializeField] bool _isClickable;

    private void Start()
    {
        StartCoroutine(RemoveKeyMapFirst());
    }

    private IEnumerator RemoveKeyMapFirst()
    {
        yield return new WaitForSeconds(_firstDelay);

        StartCoroutine(FadeOutTarget(_keyMap, _targetFadeTime));
    }

    public void Update()
    {
        if (_isClickable)
        {
            if (Input.GetKeyDown(_showKey))
            {
                if (_keyMap.activeSelf)
                    StartCoroutine(FadeOutTarget(_keyMap, _targetFadeTime));
                else
                    StartCoroutine(FadeInTarget(_keyMap, _targetFadeTime));
            }
        }
    }

    public IEnumerator FadeOutTarget(GameObject targetObject, float duration)
    {
        // Fade Out Effect

        _targetFadeTime = duration;
        _elapsedFadeTime = 0f;

        Image[] currentImages = targetObject.GetComponentsInChildren<Image>(true);

        float[] startAlphaArray = new float[currentImages.Length];
        for (int i = 0; i < currentImages.Length; i++)
            startAlphaArray[i] = currentImages[i].color.a;

        while (_elapsedFadeTime < _targetFadeTime)
        {
            _elapsedFadeTime += Time.deltaTime;
            float normalizedTime = _elapsedFadeTime / _targetFadeTime; // Normalize to 0 ~ 1

            for (int i = 0; i < currentImages.Length; i++)
            {
                Color targetColor = currentImages[i].color;
                targetColor.a = Mathf.Lerp(startAlphaArray[i], 0f, normalizedTime);
                currentImages[i].color = targetColor;
            }

            yield return null;
        }

        if (!_isClickable)
            _isClickable = true;

        targetObject.SetActive(false);

        yield return null;
    }

    public IEnumerator FadeInTarget(GameObject targetObject, float duration)
    {
        // Fade In Effect

        targetObject.SetActive(true);

        _targetFadeTime = duration;
        _elapsedFadeTime = 0f;

        Image[] currentImages = targetObject.GetComponentsInChildren<Image>(true);

        float[] startAlphaArray = new float[currentImages.Length];
        for (int i = 0; i < currentImages.Length; i++)
            startAlphaArray[i] = currentImages[i].color.a;

        while (_elapsedFadeTime < _targetFadeTime)
        {
            _elapsedFadeTime += Time.deltaTime;
            float normalizedTime = _elapsedFadeTime / _targetFadeTime; // Normalize to 0 ~ 1

            for (int i = 0; i < currentImages.Length; i++)
            {
                Color targetColor = currentImages[i].color;
                targetColor.a = Mathf.Lerp(startAlphaArray[i], 1f, normalizedTime);
                currentImages[i].color = targetColor;
            }

            yield return null;
        }

        yield return null;
    }
}
