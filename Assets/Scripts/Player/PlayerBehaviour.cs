using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : StateMachineBase
{
    [SerializeField] float _jumpQueueDuration = 0.1f;
    [SerializeField] int _maxJumpCount = 2;

    /// <summary>
    /// Smooth 효과로 전처리 된 InputState
    /// </summary>
    public InputState SmoothedInputs { get { return _inputPreprocessor.SmoothedInputs; } }
    /// <summary>
    /// InputManager.Instance.GetState() 와 동일
    /// </summary>
    public InputState RawInputs { get { return InputManager.Instance.GetState(); } }

    //TODO
    /// <summary>
    /// 플레이어의 발이 땅에 닿아 있는가?
    /// </summary>
    public bool IsGrounded { get { return _rigidbody.velocity.y == 0; } }
    public bool CanJump { get { return RemainingJumpCount > 0 && !StateIs<JumpState>(); } }
    public int RemainingJumpCount { get; set; }
    public int MaxJumpCount { get { return _maxJumpCount; } }

    public Rigidbody2D Rigidbody { get { return _rigidbody; } }


    PlayerInputPreprocessor _inputPreprocessor;
    Rigidbody2D _rigidbody;

    bool _isJumpQueued;
    float _timeAfterJumpQueued;


    private void Awake()
    {
        _inputPreprocessor = GetComponent<PlayerInputPreprocessor>();
        _rigidbody = GetComponent<Rigidbody2D>();
        
    }
    protected override void Start()
    {
        base.Start();
        InputManager.Instance.JumpPressedEvent += OnJumpPressed; //TODO : unsubscribe

        RemainingJumpCount = _maxJumpCount;
    }

    protected override void Update()
    {
        base.Update();

        if (IsGrounded)
        {
            RemainingJumpCount = _maxJumpCount;
        }
        else if (!StateIs<InAirState>())
        {
            ChangeState<InAirState>();
            
        }
        //Jump if queued
        if (_isJumpQueued)
        {
            _timeAfterJumpQueued += Time.deltaTime;
            if (_timeAfterJumpQueued > _jumpQueueDuration)
                _isJumpQueued = false;
            else if (CanJump)
            {
                if (!IsGrounded && RemainingJumpCount == _maxJumpCount)
                    RemainingJumpCount--;
                _isJumpQueued = false;
                ChangeState<JumpState>();
                return;
            }
        }
    }

    void OnJumpPressed()
    {
        _isJumpQueued = true;
        _timeAfterJumpQueued = 0f;
        Debug.Log(CurrentState);
    }
}