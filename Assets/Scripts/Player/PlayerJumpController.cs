using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerJumpController : MonoBehaviour
{
    [Header("Jump Power")]
    [SerializeField] float _groundJumpPower = 15f;
    [SerializeField] float _inAirJumpPower = 12f;
    [SerializeField] float _wallJumpPower = 6f;

    [SerializeField] float _longJumpDuration = 0.2f;
    [SerializeField] float _longJumpBonus = 3f;

    [Header("Jump Settings")]
    [SerializeField] float _coyoteTime = 0.2f;
    [SerializeField] float _jumpQueueDuration = 0.1f;
    [SerializeField] int _maxJumpCount = 2;

    public bool CanJump
    {
        get
        {
            return ((_remainingJumpCount > 0) && !_player.StateIs<JumpState>()) ||
                                        (_remainingJumpCount == _maxJumpCount && _coyoteAvailable);
        }
    }

    //public bool CanJump
    //{
    //    get
    //    {
    //        return ((_remainingJumpCount == _maxJumpCount) && _coyoteAvailable) // first jump
    //                   || (_remainingJumpCount == 1) && !_player.IsGrounded  // second jump
    //                   || (_player.IsTouchedWall) && (_remainingJumpCount == _maxJumpCount); // wall jump
    //                // || (_remainingJumpCount == _maxJumpCount) && !_player.IsGrounded; // air jump
    //    }
    //}

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
        if ((_player.IsGrounded && !_player.StateIs<JumpState>()) || _player.StateIs<WallState>())
            ResetJumpCount();

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
            // 대쉬 상태 이라면 return
            if (_player.StateIs<DashState>())
                return;

            // 벽타기 상태에서 "바라보는 방향 == 키 입력 방향" 이라면 return
            if (_player.StateIs<WallState>() && (_player.RecentDir == Mathf.RoundToInt(_player.RawInputs.Movement.x)))
            {
                _isJumpQueued = false;
                return;
            }

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
        _isJumpQueued = false;

        //if ((!_player.IsGrounded && _remainingJumpCount == _maxJumpCount) && !_coyoteAvailable) //공중점프 시 점프 차감
        //    _remainingJumpCount--;

        _isGroundJump = (_remainingJumpCount == _maxJumpCount);
        _remainingJumpCount--;
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

    public void ExecuteWallJumpAnimEvent()
    {
        _player.IsWallJump = true;

        // player input
        int xInput = (int)_player.RawInputs.Movement.x; // -1 0 1
        int yInput = (int)_player.RawInputs.Movement.y; // -1 0 1

        // sub power
        float xPower = (xInput == 0) ? 0.5f : Mathf.Abs(xInput);
        float yPower = (yInput > 0) ? 2.5f : 1f; // 위쪽키를 누르지 않으면 살짝 점프

        // player flip
        _player.RecentDir = (-1) * _player.RecentDir;
        transform.localScale = new Vector3(_player.RecentDir, transform.localScale.y, transform.localScale.z);

        // execute jump
        _player.Rigidbody.velocity = new Vector2(_player.RecentDir * xPower, yPower) * _wallJumpPower;
    }
}