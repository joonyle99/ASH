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
    
    public void StartStartingPointMovement()
    {
        StartCoroutine(MoveToStartingPointCoroutine());
    }
    public void StartCurveMovement()
    {
        _curvePath.ControlPoints[_curvePath.Length - 1].position = SceneContext.Current.Player.HandRigidBody.transform.position;
        StartCoroutine(CurveMovementCoroutine());
    }

    public void StartFlash()
    {
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator MoveToStartingPointCoroutine()
    {
        Vector3 originalPosition = transform.position;
        float eTime = 0f;
        while (eTime < _moveToStartingPointDuration)
        {
            float t = eTime / _moveToStartingPointDuration;
            transform.position = Vector3.Lerp(originalPosition, _curvePath.ControlPoints[0].position, t);
            eTime += Time.deltaTime;
            yield return null;
        }
        transform.position = _curvePath.ControlPoints[0].position;
    }
    private IEnumerator CurveMovementCoroutine()
    {
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
        eTime = 0f;
        while (eTime < _flashEndDuration)
        {
            float t = 1 - eTime / _flashEndDuration;
            _flashLight.pointLightOuterRadius = _flashMaxRadius * t;
            yield return null;
            eTime += Time.deltaTime;
        }

        // 플레이어의 Cape Material을 변경한다
        SceneContext.Current.Player.SetCapeIntensity(2.7f);

        if(_isDestroyAfterFlash)
            Destroy(gameObject);
    }
}
