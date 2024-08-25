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
        /*
        if (!PersistentDataManager.Get<bool>("keymap"))
        {
            PersistentDataManager.Set("keymap", true);
            _keyMap.SetActive(true);
            StartCoroutine(RemoveKeyMapFirst());
        }
        */
    }

    private IEnumerator RemoveKeyMapFirst()
    {
        yield return new WaitForSeconds(_firstDelay);

        StartCoroutine(FadeOut(_targetFadeTime));
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_keyMap.activeSelf)
            {
                _keyMap.SetActive(false);
                _isClickable = true;
            }
        }

        if (_isClickable)
        {
            if (Input.GetKeyDown(_showKey))
            {
                if (!_keyMap.activeSelf)
                    _keyMap.SetActive(true);
            }
        }
    }

    public IEnumerator FadeOut(float duration)
    {
        // Fade Out Effect

        _targetFadeTime = duration;
        _elapsedFadeTime = 0f;

        Image[] currentImages = _keyMap.GetComponentsInChildren<Image>(true);

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

        _keyMap.SetActive(false);

        // 원래대로 되돌리기
        for (int i = 0; i < currentImages.Length; i++)
        {
            var targetColor = currentImages[i].color;
            targetColor.a = startAlphaArray[i];
            currentImages[i].color = targetColor;
        }

        yield return null;
    }
}
