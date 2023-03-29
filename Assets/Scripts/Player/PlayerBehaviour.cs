using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : StateMachineBase
{
    /// <summary>
    /// Smooth 효과로 전처리 된 InputState
    /// </summary>
    public InputState SmoothedInputs { get { return _inputPreprocessor.SmoothedInputs; } }
    /// <summary>
    /// InputManager.Instance.GetState() 와 동일
    /// </summary>
    public InputState RawInputs { get { return InputManager.Instance.GetState(); } }

    public bool IsGrounded { get { return _rigidbody.velocity.y == 0; } }
    public int MaxJumpCount { get { return _jumpController.MaxJumpCount; } }

    public Rigidbody2D Rigidbody { get { return _rigidbody; } }


    PlayerJumpController _jumpController;
    PlayerInputPreprocessor _inputPreprocessor;
    Rigidbody2D _rigidbody;

    bool _isJumpQueued;
    float _timeAfterJumpQueued;


    private void Awake()
    {
        _inputPreprocessor = GetComponent<PlayerInputPreprocessor>();
        _jumpController = GetComponent<PlayerJumpController>();
        _rigidbody = GetComponent<Rigidbody2D>();
        
    }
    protected override void Start()
    {
        base.Start();
        InputManager.Instance.JumpPressedEvent += _jumpController.OnJumpPressed; //TODO : unsubscribe

    }

    protected override void Update()
    {
        base.Update();

        UpdateImageFlip();

        if (IsGrounded)
        {
            _jumpController.ResetJumpCount();
        }
        else //필요하다면 코요테 타임 동안은 InAir상태가 안되게 할지 결정
        {
            if (!StateIs<InAirState>())
                ChangeState<InAirState>();
        }

    }


    private void UpdateImageFlip()
    {
        int dir = 1;
        if (RawInputs.Movement.x == -1)
            dir = -1;
        transform.localScale = new Vector3(dir, transform.localScale.y, transform.localScale.z);
        //_anim.transform.rotation = left ? Quaternion.Euler(0, -90, 0) : Quaternion.Euler(0, 90, 0);
    }
}