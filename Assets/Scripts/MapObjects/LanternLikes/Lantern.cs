using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Lantern : LanternLike, ILightCaptureListener, ISceneContextBuildListener
{
    [System.Serializable]
    public struct LightSettings
    {
        public float OuterRadius;
        public float Intensity;
    }

    bool _isInsideCave = false;
    [SerializeField] bool _turnedOnAtStart = false;
    [SerializeField] float _lightUpTime = 1.5f;
    LightSettings _normalLightSettings;
    LightSettings _caveLightSettings;
    [Header("Idle Effect Settings")]
    [SerializeField] float _idleEffectScaleMin = 0.9f;
    [SerializeField] float _idleEffectScaleMax = 1.1f;
    [SerializeField] float _idleEffectIntervalMin = 1f;
    [SerializeField] float _idleEffectIntervalMax = 1.5f;
    [Header("Explode Effect Settings")]
    [SerializeField] float _maxGrowScale = 0.5f;
    [SerializeField] float _explosionIntensityScale = 1.5f;
    [SerializeField] float _explosionScale = 1.5f;
    [SerializeField] float _explosionStartDuration = 0.1f;
    [SerializeField] float _explosionEndDuration = 0.9f;
    LightSettings _currentSettings;
    [Header("References")]
    Light2D _currentSpotLight;
    [SerializeField] Light2D _normalSpotLight;
    [SerializeField] Light2D _caveSpotLight;
    [SerializeField] SoundList _soundList;

    float _currentLightFill = 0f;

    float _idleTime = 0f;
    float _idleInterval;
    bool _isExplodeDone = false;

    [SerializeField] bool _isLastAttack = false;

    PreserveState _statePreserver;

    void Awake()
    {
        _statePreserver = GetComponent<PreserveState>();
    }
    void Update()
    {
        if (!IsLightOn || (IsLightOn && !_isExplodeDone && !_turnedOnAtStart))
            return;
        _idleTime += Time.deltaTime;
        if (_idleTime > _idleInterval)
        {
            _idleTime -= _idleInterval;
            _idleInterval = Random.Range(_idleEffectIntervalMin, _idleEffectIntervalMax);
        }
        float sinValue = Mathf.Cos((_idleTime / _idleInterval) * Mathf.PI * 2);
        _currentSpotLight.pointLightOuterRadius = Mathf.Lerp(_idleEffectScaleMin, _idleEffectScaleMax, (sinValue + 1) / 2) * _currentSettings.OuterRadius;

    }
    void OnDestroy()
    {
        if (_statePreserver)
        {
            if (SceneChangeManager.Instance && SceneChangeManager.Instance.SceneChangeType == SceneChangeType.ChangeMap)
            {
                _statePreserver.SaveState("_isOn", IsLightOn);
            }

            SaveAndLoader.OnSaveStarted -= SaveLanternOnState;
        }
    }

    void TurnCurrentSpotLightOn()
    {
        _currentSpotLight.gameObject.SetActive(true);
        _currentSpotLight.pointLightOuterRadius = _currentSettings.OuterRadius;
        _currentSpotLight.intensity = _currentSettings.Intensity;
    }

    public void OnSceneContextBuilt()
    {
        var collisions = Physics2D.OverlapPointAll(LightPoint.position);
        foreach (var collision in collisions)
        {
            if (collision.GetComponent<HiddenPathDarkness>() != null)
            {
                _isInsideCave = true;
                break;
            }
        }
        if (_isInsideCave)
        {
            _caveLightSettings.OuterRadius = _caveSpotLight.pointLightOuterRadius;
            _caveLightSettings.Intensity = _caveSpotLight.intensity;
            _currentSpotLight = _caveSpotLight;
            _currentSettings = _caveLightSettings;
        }
        else
        {
            _normalLightSettings.OuterRadius = _normalSpotLight.pointLightOuterRadius;
            _normalLightSettings.Intensity = _normalSpotLight.intensity;
            _currentSpotLight = _normalSpotLight;
            _currentSettings = _normalLightSettings;
        }
        _idleInterval = Random.Range(_idleEffectIntervalMin, _idleEffectIntervalMax);
        if (!_turnedOnAtStart)
        {
            _currentSpotLight.intensity = 0;
            _currentSpotLight.pointLightOuterRadius = 0;
        }
        else
        {
            TurnCurrentSpotLightOn();
            IsLightOn = true;
        }

        if (_statePreserver)
        {
            if (SceneChangeManager.Instance && SceneChangeManager.Instance.SceneChangeType == SceneChangeType.Loading)
            {
                if (_statePreserver.LoadState("_isOnSaved", false))
                {
                    TurnOnImmediately();
                }
            }
            else
            {
                if (_statePreserver.LoadState("_isOn", false))
                {
                    TurnOnImmediately();
                }
            }
        }

        SaveAndLoader.OnSaveStarted += SaveLanternOnState;
    }

    void TurnLightOn()
    {
        if (IsLightOn)
            return;

        IsLightOn = true;

        if (!_isExplodeDone)
        {
            StartCoroutine(ExplodeCoroutine());
        }
    }
    void TurnLightOff()
    {
        if (!IsLightOn)
            return;

        _currentSpotLight.gameObject.SetActive(false);

        IsLightOn = false;

        _isExplodeDone = false;

        StopAllCoroutines();
    }
    IEnumerator ExplodeCoroutine()
    {
        // 보스 랜턴 공격 판단
        LanternSceneContext lightSceneContext = LanternSceneContext.Current;
        bool isBossScene = lightSceneContext != null && lightSceneContext.Boss != null;
        if (isBossScene == true)
        {
            // 공격을 위한 설정
            InputManager.Instance.ChangeToStayStillSetter();
            SceneContext.Current.Player.IsGodMode = true;
        }

        float eTime = 0f;
        float originalRadius = _currentSpotLight.pointLightOuterRadius;
        float originalIntensity = _currentSpotLight.intensity;

        _soundList.PlaySFX("SE_Lantern_Work");

        while (eTime < _explosionStartDuration)
        {
            float t = (eTime / _explosionStartDuration);
            _currentSpotLight.pointLightOuterRadius = Mathf.Lerp(originalRadius, _currentSettings.OuterRadius * _explosionScale, t);
            _currentSpotLight.intensity = Mathf.Lerp(originalIntensity, originalIntensity * _explosionIntensityScale, t);
            yield return null;
            eTime += Time.deltaTime;
        }

        eTime = 0f;

        while (eTime < _explosionEndDuration)
        {
            float t = (eTime / _explosionEndDuration);
            _currentSpotLight.pointLightOuterRadius = Mathf.Lerp(_currentSettings.OuterRadius * _explosionScale, _currentSettings.OuterRadius, Utils.Curves.EaseOut(t));
            _currentSpotLight.intensity = Mathf.Lerp(originalIntensity * _explosionIntensityScale, originalIntensity, Utils.Curves.EaseOut(t));
            yield return null;
            eTime += Time.deltaTime;
        }

        _currentSpotLight.pointLightOuterRadius = _currentSettings.OuterRadius;
        _isExplodeDone = true;

        // 보스 랜턴 공격 실행
        if (isBossScene == true)
        {
            var lanternAttack = new LanternAttack(this, lightSceneContext.Boss);
            StartCoroutine(lightSceneContext.LenternAttack(lanternAttack, _isLastAttack));
        }
    }

    public void OnDarkBeamCollision()
    {
        TurnLightOff();
        _currentLightFill = 0f;
    }

    public void OnLightEnter(LightCapturer capturer, LightSource lightSource)
    {
        if (IsLightOn)
            return;
        _currentSpotLight.gameObject.SetActive(true);
    }
    public void OnLightStay(LightCapturer capturer, LightSource lightSource)
    {
        if (IsLightOn)
            return;
        _currentLightFill += Time.deltaTime;

        _currentSpotLight.pointLightOuterRadius = (_currentLightFill / _lightUpTime) * _currentSettings.OuterRadius * _maxGrowScale;
        _currentSpotLight.intensity = (_currentLightFill / _lightUpTime) * _currentSettings.Intensity;
        if (_currentLightFill > _lightUpTime)
        {
            TurnLightOn();
        }
    }
    public void OnLightExit(LightCapturer capturer, LightSource lightSource)
    {
        if (IsLightOn)
            return;
        _currentLightFill = 0f;
        _currentSpotLight.gameObject.SetActive(false);
    }

    private void TurnOnImmediately()
    {
        _isExplodeDone = true;
        TurnCurrentSpotLightOn();
        IsLightOn = true;
    }
    private void SaveLanternOnState()
    {
        if(_statePreserver)
            _statePreserver.SaveState("_isOnSaved", IsLightOn);
    }
}
