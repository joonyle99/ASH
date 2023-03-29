using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpController : MonoBehaviour
{
    [Header("Jump Power")]
    [SerializeField] float _groundJumpPower = 9f;
    [SerializeField] float _inAirJumpPower = 9f;

    [SerializeField] float _longJumpDuration = 0.3f;
    [SerializeField] float _longJumpBonus = 0.3f;

    [Header("Jump Settings")]
    [SerializeField] float _coyoteTime = 0.2f;

    [SerializeField] float _jumpQueueDuration = 0.1f;
    [SerializeField] int _maxJumpCount = 2;

    public bool CanJump { get { return (_remainingJumpCount > 0 && !_player.StateIs<JumpState>())
                                    || _coyoteAvailable; } }
    public int MaxJumpCount { get { return _maxJumpCount; } }


    bool _coyoteAvailable { get { return _timeAfterGroundLeft <= _coyoteTime; } }

    bool _isLongJumping;
    float _longJumpTime = 0f;

    int _remainingJumpCount;
    bool _isGroundJump;

    bool _isJumpQueued;
    float _timeAfterJumpQueued;

    float _timeAfterGroundLeft;

    PlayerBehaviour _player;
    void Awake()
    {
        _remainingJumpCount = _maxJumpCount;
        _player = GetComponent<PlayerBehaviour>();
    }
    void Update()
    {
        //Process Long jump (롱점프 시간 동안은 위쪽으로 힘을 더 줌)
        if (_isLongJumping)
        {
            _player.Rigidbody.velocity -= _longJumpBonus * Physics2D.gravity * Time.deltaTime;
            _longJumpTime += Time.deltaTime;
            if (_longJumpTime > _longJumpDuration || !_player.RawInputs.IsPressingJump)
                _isLongJumping = false;
        }

        //Coyote Jump
        if(_player.IsGrounded)
            _timeAfterGroundLeft = 0f;
        else
            _timeAfterGroundLeft += Time.deltaTime;

        //Jump if queued
        if (_isJumpQueued)
        {
            _timeAfterJumpQueued += Time.deltaTime;
            if (_timeAfterJumpQueued > _jumpQueueDuration)
                _isJumpQueued = false;
            else if (CanJump)
            {
                CastJump();
                return;
            }
        }

    }

    public void ResetJumpCount()
    {
        _remainingJumpCount = _maxJumpCount;
    }

    public void OnJumpPressed()
    {
        _isJumpQueued = true;
        _timeAfterJumpQueued = 0f;
    }
    
    //JumpState 시작
    void CastJump()
    {
        if ((!_player.IsGrounded && _remainingJumpCount == _maxJumpCount) && !_coyoteAvailable) //공중점프 시 점프 차감
            _remainingJumpCount--;
        _isJumpQueued = false;

        _isGroundJump = _remainingJumpCount == _maxJumpCount;
        _remainingJumpCount -= 1;
        _isLongJumping = true;
        _longJumpTime = 0f;
        _player.ChangeState<JumpState>();
    }

    //점프 애니메이션 종료 시점
    public void ExecuteJumpAnimEvent()
    {
        float jumpPower = _isGroundJump ? _groundJumpPower : _inAirJumpPower;
        _player.Rigidbody.velocity = new Vector2(_player.Rigidbody.velocity.x, jumpPower);
    }
}