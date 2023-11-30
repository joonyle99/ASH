using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LightBeamLineEffect : MonoBehaviour
{
    public Vector3 CurrentShootingPosition { get; private set; }
    public bool IsShootingDone { get { return _isShootingDone;} }
    [SerializeField] LineRenderer _lineRenderer;

    [SerializeField] float _lineDrawSpeed = 30f;
    [SerializeField] float _lastLineDrawSpeed = 10f;
    [SerializeField] float _lineIdleIntensityMax = 1.2f;
    [SerializeField] float _lineIdleIntensityMin = 0.6f;
    [SerializeField] float _lineIdleEffectInterval= 1f;

    Transform[] _connectedTransforms = null;  


    Coroutine _shootingCoroutine = null;
    bool _isShootingDone = false;

    float _idleTime = 0f;

    const float MinDistanceFromLantern = 0.01f;
    public void MarkAsLastConnection()
    {
        _lineDrawSpeed = _lastLineDrawSpeed;
    }
    private void Update()
    {
        if (_connectedTransforms != null)
        {
            for (int i = 0; i < _lineRenderer.positionCount; i++)
                _lineRenderer.SetPosition(i, _connectedTransforms[i].position);
            if (_isShootingDone)
            {
                _idleTime = (_idleTime + Time.deltaTime) % _lineIdleEffectInterval;
                //sin 파형으로 intensity 조정
                float sinValue = Mathf.Sin((_idleTime / _lineIdleEffectInterval) * Mathf.PI * 2);
                _lineRenderer.material.SetFloat("_Intensity", Mathf.Lerp(_lineIdleIntensityMin, _lineIdleIntensityMax, (sinValue + 1)/2));
            }
        }

    }
    public void StartBeamEffect(Transform[] targetTransforms)
    {
        _connectedTransforms = targetTransforms;
        _shootingCoroutine = StartCoroutine(AnimateLine());
    }
    private void OnDisable()
    {
        _connectedTransforms = null;
        _isShootingDone = false;
        if (_shootingCoroutine != null)
            StopCoroutine(_shootingCoroutine);
    }
    private IEnumerator AnimateLine()
    {
        if (_connectedTransforms.Length < 2)
        {
            Debug.LogError("Trying to draw a LightBeam with less than 2 points");
            yield break;
        }

        _lineRenderer.positionCount = 2;
        int targetPointIndex = 1;
        Vector3 currentPosition = _connectedTransforms[0].position;

        _isShootingDone = false;
        while (targetPointIndex < _connectedTransforms.Length)
        {
            Vector3 targetPosition = _connectedTransforms[targetPointIndex].position;
            Vector3 direction = (targetPosition - currentPosition).normalized;
            currentPosition += direction * _lineDrawSpeed * Time.deltaTime;
            if ((currentPosition - targetPosition).sqrMagnitude < MinDistanceFromLantern * MinDistanceFromLantern)
            {
                _lineRenderer.SetPosition(targetPointIndex, targetPosition);
                currentPosition = targetPosition;
                targetPointIndex++;
                if (targetPointIndex < _connectedTransforms.Length)
                {
                    _lineRenderer.positionCount++;
                    _lineRenderer.SetPosition(targetPointIndex, currentPosition);
                }
            }
            else
            {
                _lineRenderer.SetPosition(targetPointIndex, currentPosition);
            }
            CurrentShootingPosition = currentPosition;
            yield return null;
        }
        _isShootingDone = true;

    }
    float[] GetAccumulatedDistances(Vector3[] positions)
    {
        float[] result = new float[positions.Length];
        result[0] = 0;
        for (int i = 1; i < positions.Length; i++)
            result[i] = result[i-1] + Vector3.Distance(positions[i], positions[i + 1]);
        return result;
    }
}
