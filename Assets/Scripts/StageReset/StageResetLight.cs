using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StageResetLight : MonoBehaviour
{
    [SerializeField] private float _flashMaxRadius = 60f;
    [SerializeField] private float _flashStartDuration = 0.1f;
    [SerializeField] private float _flashDuration = 0.4f;
    [SerializeField] private float _flashEndDuration = 0.5f;
    [SerializeField] private Light2D _flashLight;

    public void StartFlash()
    {
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        float eTime = 0f;
        while (eTime < _flashStartDuration)
        {
            float t = eTime / _flashStartDuration;
            _flashLight.pointLightOuterRadius = _flashMaxRadius * t;
            yield return null;
            eTime += Time.deltaTime;
        }
        yield return new WaitForSeconds(_flashDuration);

        SceneContext.Current.Player.SetCapeIntensity(2.7f);
        eTime = 0f;
        while (eTime < _flashEndDuration)
        {
            float t = 1 - eTime / _flashEndDuration;
            _flashLight.pointLightOuterRadius = _flashMaxRadius * t;
            yield return null;
            eTime += Time.deltaTime;
        }
    }
}
