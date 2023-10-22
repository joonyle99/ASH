using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputPreprocessor : MonoBehaviour
{
    [SerializeField] float _inputAdaptSpeed = 3f;
    [SerializeField] float _inputAdaptSpeedInAir = 1.5f;


    /// <summary>
    /// InputState.Movement.x�� 0~1�� Smooth �� InputState
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
        if (_player.Rigidbody.velocity.x * inputs.Movement.x < 0) //���� �ӵ��� �Է� ������ �ݴ��� �� �����ӵ� ����
            adaptSpeed *= 3;

        float movementX = Mathf.MoveTowards(_smoothedInputs.Movement.x, inputs.Movement.x, adaptSpeed * Time.deltaTime);

        _smoothedInputs = inputs;
        _smoothedInputs.Movement.x = movementX;
    }
}

// TODO : �ν����Ϳ� input / smoothed input ǥ��