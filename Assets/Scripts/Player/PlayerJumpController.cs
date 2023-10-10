using UnityEngine;

public class PlayerJumpController : MonoBehaviour
{
    [Header("Jump Power Settings")]

    [Space]

    [Range(0f, 30f)] [SerializeField] float _groundJumpPower = 15f;
    [Range(0f, 30f)] [SerializeField] float _inAirJumpPower = 12f;
    [Range(0f, 30f)] [SerializeField] float _wallJumpPower = 6f;
    [Range(0f, 60f)] [SerializeField] float _wallEndJumpPower = 30f;

    [Range(0f, 1f)] [SerializeField] float _longJumpDuration = 0.2f;
    [Range(0f, 100f)] [SerializeField] float _longJumpBonusPower = 190f;

    [Header("Jump Settings")]

    [Space]

    [Range(0f, 10f)] [SerializeField] float _coyoteTime = 2f;
    [Range(0f, 1f)] [SerializeField] float _jumpQueueDuration = 0.1f;
    [Range(0, 10)] [SerializeField] int _maxJumpCount = 2;

    public bool CanJump { get { return _remainingJumpCount > 0 && !_player.StateIs<JumpState>() && CoyoteAvailable; } }
    public bool CoyoteAvailable { get { return _timeAfterGroundLeft <= _coyoteTime; } }

    bool _isLongJumping;
    float _longJumpTime;

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
        // Reset jump count
        if (_player.StateIs<IdleState>() || _player.StateIs<WallState>())
            ResetJumpCount();

        // Long jump (롱점프 시간 동안은 위쪽으로 힘을 더 줌)
        if (_isLongJumping)
        {
            // _player.Rigidbody.velocity -= _longJumpBonusPower * Physics2D.gravity * Time.deltaTime;
            _player.Rigidbody.AddForce(_longJumpBonusPower * (-1) * Physics2D.gravity * Time.deltaTime);

            _longJumpTime += Time.deltaTime;

            if ((_longJumpTime >= _longJumpDuration) || !_player.RawInputs.IsPressingJump)
                _isLongJumping = false;
        }

        // Jump time check
        if(_player.IsGrounded || _player.StateIs<WallState>())
            _timeAfterGroundLeft = 0f;
        else
            _timeAfterGroundLeft += Time.deltaTime; // 공중에 떠있는 시간

        // Jump if queued
        if (_isJumpQueued)
        {
            // 대쉬 상태 or 급강하 상태면 종료
            if (_player.StateIs<DashState>() || _player.StateIs<DiveState>() || _player.StateIs<ShootingState>() || _player.StateIs<HurtState>() || _player.StateIs<DieState>() || _player.StateIs<HealingState>())
            {
                _isJumpQueued = false;
                return;
            }

            // 벽타기 상태에서 "바라보는 방향 == 키 입력 방향" 이라면 종료
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
        _isGroundJump = (_remainingJumpCount == _maxJumpCount);
        _remainingJumpCount--;
        _isLongJumping = true;
        _longJumpTime = 0f;

        if (_player.StateIs<InAirState>())
            _player.Animator.SetTrigger("DoubleJump");

        _player.ChangeState<JumpState>();

        return;
    }

    /// <summary>
    /// Basic Jump
    /// </summary>
    public void ExecuteJumpAnimEvent()
    {
        float jumpPower = _isGroundJump ? _groundJumpPower : _inAirJumpPower;

        // ForceMode2D.Force : 충격파와 같은 힘을 가한다
        _player.Rigidbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Wall Jump
    /// </summary>
    public void ExecuteWallJumpAnimEvent()
    {
        _player.IsWallJump = true;

        // player input
        int xInput = (int)_player.RawInputs.Movement.x; // -1 0 1
        int yInput = (int)_player.RawInputs.Movement.y; // -1 0 1

        // sub power
        float xPower = (xInput == 0) ? 0.7f : Mathf.Abs(xInput);    // 좌우 방향키를 누르지 않으면 살짝 점프
        float yPower = (yInput > 0) ? 2.5f : 1f;                    // 위쪽키를 누르지 않으면 살짝 점프

        // player flip
        _player.RecentDir = (-1) * _player.RecentDir;
        transform.localScale = new Vector3(_player.RecentDir, transform.localScale.y, transform.localScale.z);

        // execute jump
        _player.Rigidbody.velocity = new Vector2(_player.RecentDir * xPower, yPower) * _wallJumpPower;
    }

    /// <summary>
    /// End Wall Jump
    /// </summary>
    public void ExcuteEndWallJumpAnimEvent()
    {
        _remainingJumpCount--;

        _player.Rigidbody.velocity =
            new Vector2(_player.Rigidbody.velocity.x, _wallEndJumpPower);

    }
}