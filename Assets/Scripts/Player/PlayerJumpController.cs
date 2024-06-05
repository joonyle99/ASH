using System.Collections;
using UnityEngine;

public class PlayerJumpController : MonoBehaviour
{
    [Header("Jump Settings")]

    [Space]

    [SerializeField] private float _startJumpPower = 14f;
    [SerializeField] private float _inAirJumpPower = 14f;

    [Space]

    [SerializeField] private float _wallJumpPower = 4f;
    [SerializeField] private float _wallEndJumpPower = 18f;

    [Space]

    [SerializeField] private float _longJumpPower = 4f;
    [SerializeField] private float _longJumpDuration = 0.2f;

    [Space]

    [SerializeField] private float _jumpQueueDuration = 0.1f;
    [SerializeField] private float _coyoteTime = 3f;
    [SerializeField] private int _maxJumpCount = 2;

    [Header("Effects")]

    [SerializeField] private ParticleHelper _doubleJumpEffect;
    [SerializeField] private ParticleHelper _jumpTrailEffect;

    // Properties
    public bool CanJump => _remainingJumpCount > 0 && _timeAfterPlatformLeft <= _coyoteTime;
    public int MaxJumpCount => PersistentDataManager.GetByGlobal<bool>("DoubleJump") ? _maxJumpCount : 1;

    private bool _isJumpKeyQueued;
    private bool _isStartJump;
    private int _remainingJumpCount;
    private bool _isLongJumping;
    private float _longJumpTime;
    private float _timeAfterPlatformLeft;

    private Coroutine _jumKeyTimerCoroutine;
    private PlayerBehaviour _player;

    private void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
    }
    private void Start()
    {
        ResetJumpCount();
    }
    private void FixedUpdate()
    {
        // Long jump (롱점프 시간 동안은 위쪽으로 힘을 더 줌)
        // (_isLongJumping && _remainingJumpCount >= MaxJumpCount - 1)
        if (_isLongJumping)
            _player.Rigidbody.AddForce(_longJumpPower * (-1) * Physics2D.gravity);
    }
    private void Update()
    {
        // Enqueue jump input
        if (InputManager.Instance.State.JumpKey.KeyDown)
            StartJumpQueueCoroutine();

        // Reset jump count
        if(_remainingJumpCount < MaxJumpCount)
        {
            if (((_player.CurrentStateIs<IdleState>() || _player.CurrentStateIs<RunState>()) && _player.IsGrounded) || _player.CurrentStateIs<WallState>())
            {
                ResetJumpCount();
            }
        }

        // Check left time after platform
        if (_player.IsGrounded || _player.CurrentStateIs<WallState>()) _timeAfterPlatformLeft = 0f;
        else _timeAfterPlatformLeft += Time.deltaTime;

        // Check long jump time
        if (_isLongJumping)
        {
            _longJumpTime += Time.deltaTime;

            // 롱점프 시간이 지나거나 점프 버튼을 때면 롱점프는 종료된다.
            if (_longJumpTime >= _longJumpDuration || !InputManager.Instance.State.JumpKey.Pressing)
            {
                _longJumpTime = 0f;
                _isLongJumping = false;
            }
        }

        // Is there any jump input in the queue?
        if (!_isJumpKeyQueued) return;

        // Player의 Current State가 Jump State로 진입할 수 없는 상태라면 점프 불가
        if (_player.CurrentState is not IJumpableState)
        {
            _isJumpKeyQueued = false;
            return;
        }

        // 벽타기 상태에서 바라보는 방향과 반대 방향으로 방향키를 누르지 않으면 점프 x
        if (_player.CurrentStateIs<WallState>() && !_player.IsOppositeDirSync)
        {
            _isJumpKeyQueued = false;
            return;
        }

        // cast jump
        if (CanJump)
            CastJump();
    }

    public void ResetJumpCount()
    {
        _remainingJumpCount = MaxJumpCount;
    }
    private void CastJump()
    {
        _isJumpKeyQueued = false;
        _isLongJumping = true;
        _isStartJump = _remainingJumpCount == MaxJumpCount;
        _remainingJumpCount--;

        _jumpTrailEffect.Emit(2);

        if (!_isStartJump)
        {
            _doubleJumpEffect.Emit(1);
            _player.Animator.SetTrigger("DoubleJump");
        }

        _player.ChangeState<JumpState>();
    }
    public void StartJumpQueueCoroutine()
    {
        if (_jumKeyTimerCoroutine != null)
            StopCoroutine(_jumKeyTimerCoroutine);

        _jumKeyTimerCoroutine = StartCoroutine(JumpQueueCoroutine());
    }
    private IEnumerator JumpQueueCoroutine()
    {
        // push jump input to 'jump queue'
        _isJumpKeyQueued = true;

        yield return new WaitForSeconds(_jumpQueueDuration);

        // pop jump input to 'jump queue'
        _isJumpKeyQueued = false;
    }

    /// <summary>
    /// Basic Jump
    /// </summary>
    public void BasicJump()
    {
        var jumpPower = _isStartJump ? _startJumpPower : _inAirJumpPower;

        // 현재의 velocity.y를 초기화 시킨다
        _player.Rigidbody.velocity = new Vector2(_player.Rigidbody.velocity.x, 0f);

        // ForceMode2D.Force : 충격파와 같은 힘을 가한다 (Jump를 위한 출력방식)
        _player.Rigidbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    }
    /// <summary>
    /// Wall Jump
    /// </summary>
    public void WallJump()
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
    public void EndWallJump()
    {
        // 한번에 2개의 점프가 깎인다.
        _remainingJumpCount = 0;

        Vector2 endWallJumpForce = new Vector2(_player.Rigidbody.velocity.x, _wallEndJumpPower);

        _player.Rigidbody.AddForce(endWallJumpForce, ForceMode2D.Impulse);

    }
}