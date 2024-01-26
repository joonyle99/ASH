using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinMovementAnim : MonoBehaviour
{
    [SerializeField] float _frequency = 1f;
    [SerializeField] float _movementAmount = 0.5f;

    Vector3 originalPosition;
    float eTime = 0f;
    private void Awake()
    {
        originalPosition = transform.localPosition;
    }
    private void Update()
    {
        eTime += Time.deltaTime;
        transform.localPosition = originalPosition + new Vector3(0, Mathf.Sin(eTime * _frequency * Mathf.PI) * _movementAmount/2, 0);
    }
}
