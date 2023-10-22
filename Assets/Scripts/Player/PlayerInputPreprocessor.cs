using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputPreprocessor : MonoBehaviour
{
    [SerializeField] float _inputAdaptSpeed = 3f;
    [SerializeField] float _inputAdaptSpeedInAir = 1.5f;


    /// <summary>
    /// InputState.Movement.x를 0~1로 Smooth 한 InputState
    /// </summary>
    public InputState SmoothedInputs { get { return _smoothedInputs; } }

    InputState _smoothedInputs;
    PlayerBehaviour _player;

    private void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
    }
    void Update()
    {
        InputState inputs = InputManager.Instance.GetState();

        float adaptSpeed = _player.IsGrounded ? _inputAdaptSpeed : _inputAdaptSpeedInAir;
        if (_player.Rigidbody.velocity.x * inputs.Movement.x < 0) //현재 속도와 입력 방향이 반대일 때 반응속도 증폭
            adaptSpeed *= 3;

        float movementX = Mathf.MoveTowards(_smoothedInputs.Movement.x, inputs.Movement.x, adaptSpeed * Time.deltaTime);

        _smoothedInputs = inputs;
        _smoothedInputs.Movement.x = movementX;
    }
}

// TODO : 인스펙터에 input / smoothed input 표시