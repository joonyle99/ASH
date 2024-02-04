using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerJumpController : MonoBehaviour
{
    [Header("Jump Power Settings")]

    [Space]

    [SerializeField] float _startJumpPower = 14f;
    [SerializeField] float _inAirJumpPower = 12f;
    [SerializeField] float _wallJumpPower = 6f;
    [SerializeField] float _wallEndJumpPower = 15f;

    [SerializeField] float _longJumpDuration = 0.2f;
    [SerializeField] float _longJumpPower = 4f;

    [Header("Jump Settings")]

    [Space]

    [SerializeField] float _coyoteTime = 3f;
    [SerializeField] float _jumpQueueDuration = 0.1f;
    [SerializeField] int _maxJumpCount = 2;

    [SerializeField] bool _canJump;
    [SerializeField] bool _coyoteAvailable;

    [Header("Effects")]

    [SerializeField] ParticleHelper _doubleJumpEffect;
    [SerializeField] ParticleHelper _jumpTrailEffect;

    // Properties
    public bool CanJump => _canJump;
    public bool CoyoteAvailable => _coyoteAvailable;

    // Variables
    bool _isLongJumping;
    float _longJumpTime;

    int _remainingJumpCount;
    bool _isStartJump;

    bool _isJumpQueued;
    float _timeAfterJumpQueued;

    float _timeAfterPlatformLeft;

    PlayerBehaviour _player;

    void Awake()
    {
        _remainingJumpCount = _maxJumpCount;
        _player = GetComponent<PlayerBehaviour>();
    }

    void FixedUpdate()
    {
        // Long jump (롱점프 시간 동안은 위쪽으로 힘을 더 줌)
        if (_isLongJumping && _remainingJumpCount >= _maxJumpCount - 1)
        {
            // Debug.Log("롱 점프 되는중 ~~");
            _player.Rigidbody.AddForce(_longJumpPower * (-1) * Physics2D.gravity);
        }
    }

    void Update()
    {
        // TEMP
        if (!PersistentDataManager.Get<bool>("DoubleJump"))
            _maxJumpCount = 1;
        else
            _maxJumpCount = 2;

        if (InputManager.Instance.State.JumpKey.KeyDown)
        {
            OnJumpPressed();
        }
        _coyoteAvailable = (_timeAfterPlatformLeft <= _coyoteTime);
        _canJump = (_remainingJumpCount > 0 && _coyoteAvailable);

        // Reset jump count
        if (_player.CurrentStateIs<IdleState>() || _player.CurrentStateIs<WallState>())
            ResetJumpCount();

        // Long jump (롱점프 시간 동안은 위쪽으로 힘을 더 줌)
        if (_isLongJumping)
        {
            _longJumpTime += Time.deltaTime;

            // 롱점프 시간이 지나거나 점프 버튼을 때면 롱점프는 종료된다.
            if ((_longJumpTime >= _longJumpDuration) || !InputManager.Instance.State.JumpKey.Pressing)
                _isLongJumping = false;
        }

        // Jump time check
        if(_player.IsGrounded || _player.CurrentStateIs<WallState>())
            _timeAfterPlatformLeft = 0f;
        else
            _timeAfterPlatformLeft += Time.deltaTime; // 발판을 떠난 후 경과 시간 증가

        // Jump if queued
        if (_isJumpQueued)
        {
            // Player의 Current State가 Jump State로 진입할 수 없는 상태라면 점프 불가
            if (_player.CurrentStateIs<DashState>() || _player.CurrentStateIs<DiveState>() || _player.CurrentStateIs<ShootingState>() || _player.CurrentStateIs<HurtState>() || _player.CurrentStateIs<DieState>() || _player.CurrentStateIs<HealingState>())
            {
                _isJumpQueued = false;
                return;
            }

            // 벽타기 상태에서 바라보는 방향과 반대 방향으로 방향키를 누르지 않으면 점프 x
            if (_player.CurrentStateIs<WallState>() && !_player.IsOppositeDirSync)
            {
                _isJumpQueued = false;
                return;
            }

            _timeAfterJumpQueued += Time.deltaTime;

            // 점프키가 눌린 후 JumpQueue에서 점프 명령이 대기하는데, 대기 시간이 종료되면 해당 점프 명령은 취소된다.
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
        _isLongJumping = true;
        _longJumpTime = 0f;
        _isStartJump = (_remainingJumpCount == _maxJumpCount);
        _remainingJumpCount--;

        _jumpTrailEffect.Emit(2);
        if (_player.CurrentStateIs<InAirState>())
        {
            _doubleJumpEffect.Emit(1);
            _player.Animator.SetTrigger("DoubleJump");
        }

        _player.ChangeState<JumpState>();
    }

    /// <summary>
    /// Basic Jump
    /// </summary>
    public void ExcuteBasicJump()
    {
        float jumpPower = _isStartJump ? _startJumpPower : _inAirJumpPower;

        // 현재의 velocity.y를 초기화 시킨다
        _player.Rigidbody.velocity = new Vector2(_player.Rigidbody.velocity.x, 0f);

        // ForceMode2D.Force : 충격파와 같은 힘을 가한다 (Jump를 위한 출력방식)
        _player.Rigidbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Wall Jump
    /// </summary>
    public void ExecuteWallJump()
    {
        _player.IsClimbJump = true;

        // player input
        int xInput = (int)_player.RawInputs.Movement.x; // -1 0 1
        int yInput = (int)_player.RawInputs.Movement.y; // -1 0 1

        // sub power
        float xPower = (xInput == 0) ? 0.7f : Mathf.Abs(xInput);    // 좌우 방향키를 누르지 않으면 살짝 점프
        float yPower = (yInput > 0) ? 2.5f : 1.5f;                  // 위쪽키를 누르지 않으면 살짝 점프

        // player flip
        _player.RecentDir = (-1) * _player.RecentDir;
        transform.localScale = new Vector3(_player.RecentDir, transform.localScale.y, transform.localScale.z);

        // execute jump
        Vector2 wallJumpForce = new Vector2(_player.RecentDir * xPower, yPower) * _wallJumpPower;
        _player.Rigidbody.AddForce(wallJumpForce, ForceMode2D.Impulse);
    }

    /// <summary>
    /// End Wall Jump
    /// </summary>
    public void ExcuteEndWallJump()
    {
        // 한번에 2개의 점프가 깎인다.
        _remainingJumpCount = 0;

        Vector2 endWallJumpForce = new Vector2(_player.Rigidbody.velocity.x, _wallEndJumpPower);

        _player.Rigidbody.AddForce(endWallJumpForce, ForceMode2D.Impulse);

    }
}