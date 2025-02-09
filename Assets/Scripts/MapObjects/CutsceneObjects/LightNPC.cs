using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightNPC : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private bool _isDestroyAfterFlash = true;

    [SerializeField] private BezierCurvePath _curvePath;
    [SerializeField] private float _moveToStartingPointDuration = 1f;
    [SerializeField] private float _curveMovementDuration = 2f;
    [SerializeField] private float _flashMaxRadius = 60f;
    [SerializeField] private float _flashStartDuration = 0.1f;
    [SerializeField] private float _flashDuration = 0.4f;
    [SerializeField] private float _flashEndDuration = 0.5f;
    [SerializeField] private Light2D _flashLight;
    [SerializeField] private float _playerCapeIntensityValue = 0f;
    
    private SoundList _soundList;
    private AudioSource _loopSound;

    private void Awake()
    {
        _soundList = GetComponent<SoundList>();
        _loopSound = GetComponent<AudioSource>();
    }

    public void SetParentToNull()
    {
        gameObject.transform.SetParent(null);
    }

    public void StartStartingPointMovement()
    {
        StartCoroutine(MoveToStartingPointCoroutine());
    }
    private IEnumerator MoveToStartingPointCoroutine()
    {
        Vector3 originalPosition = transform.position;
        float eTime = 0f;
        while (eTime < _moveToStartingPointDuration)
        {
            float t = eTime / _moveToStartingPointDuration;
            transform.position = Vector3.Lerp(originalPosition, _curvePath.ControlPoints[0].position, t);
            yield return null;
            eTime += Time.deltaTime;
        }
        transform.position = _curvePath.ControlPoints[0].position;
        _soundList.PlaySFX("SE_Light");
    }

    public void StartCurveMovement()
    {
        _curvePath.ControlPoints[_curvePath.Length - 1].position = SceneContext.Current.Player.HandRigidBody.transform.position;
        StartCoroutine(CurveMovementCoroutine());
    }
    private IEnumerator CurveMovementCoroutine()
    {
        _soundList.StopRecentLoopPlayedSFX();

        _soundList.PlaySFX("SE_Light_move");
        float eTime = 0f;
        while (eTime < _curveMovementDuration)
        {
            eTime += Time.deltaTime;
            float t = eTime / _curveMovementDuration;
            transform.position = _curvePath.CalculateBezierPoint(Utils.Curves.EaseIn(t));
            yield return null;
        }
        yield return FlashCoroutine();
    }

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
        _soundList.PlaySFX("SE_Light_absorb");
        SceneContext.Current.Player.SetCapeIntensity(_playerCapeIntensityValue);
        eTime = 0f;
        while (eTime < _flashEndDuration)
        {
            float t = 1 - eTime / _flashEndDuration;
            _flashLight.pointLightOuterRadius = _flashMaxRadius * t;
            yield return null;
            eTime += Time.deltaTime;
        }

        if (_isDestroyAfterFlash)
        {
            DestructEventCaller destructEventCaller;

            if (TryGetComponent(out destructEventCaller))
            {
                Destruction.Destruct(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
