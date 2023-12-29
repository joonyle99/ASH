using UnityEngine;

public class PlayerCoolTimeManager : MonoBehaviour
{
    private PlayerBehaviour player;
    private DashState dashState;

    [Header("Dash")]
    [Space]

    [SerializeField] private float _targetDashCoolTime = 0.3f;
    [SerializeField] private float _elapsedDashCoolTime;

    void Awake()
    {
        player = GetComponent<PlayerBehaviour>();
        dashState = GetComponent<DashState>();
    }

    void Start()
    {

    }

    void Update()
    {
        if (!dashState.IsDashing && !player.CanDash)
        {
            _elapsedDashCoolTime += Time.deltaTime;

            if (_elapsedDashCoolTime > _targetDashCoolTime)
            {
                if (player.IsGrounded || player.StateIs<WallState>())
                {
                    _elapsedDashCoolTime = 0f;
                    player.CanDash = true;
                }
            }
        }
    }
}
