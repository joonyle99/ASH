using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueVisualEffect : MonoBehaviour, ITriggerListener
{
    [Header("Model")]

    [SerializeField]
    private Transform _model;
    [SerializeField]
    private Transform _eyes;

    private void Awake()
    {
        SaveAndLoader.OnSaveStarted += ActiveEyes;
    }

    private void ActiveEyes()
    {
        if (_eyes == null)
            return;

        SpriteRenderer leftEye = _eyes.Find("LeftEye").GetComponent<SpriteRenderer>();
        leftEye.enabled = true;

        SpriteRenderer rightEye = _eyes.Find("RightEye").GetComponent<SpriteRenderer>();
        rightEye.enabled = true;
    }
}
