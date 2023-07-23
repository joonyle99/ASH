using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [Header("Attack Setting")]

    [Space]

    [SerializeField] Transform _basicAttackHitbox;                              // 공격 타격 박스
    [Range(0f, 5f)] [SerializeField] float _attackCountRefreshTime = 1.5f;      // 공격 초기화 시간

    PlayerBehaviour _player;

    int _basicAttackCount;              // 공격 카운트
    float _timeAfterLastBasicAttack;    // 마지막으로 공격한 시간

    public bool IsBasicAttacking { get; private set; }

    private void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
    }

    public void CastBasicAttack()
    {
        _basicAttackHitbox.gameObject.SetActive(true);

        _timeAfterLastBasicAttack = Time.time;
        _basicAttackCount++;

        _player.Animator.SetTrigger("Attack");
        _player.Animator.SetBool("IsAttack", true);
        _player.Animator.SetInteger("BasicAttackCount", _basicAttackCount);

        // TODO : 기본 공격 사운드 Once 재생

        if (_basicAttackCount >= 6)
            _basicAttackCount = 0;

        IsBasicAttacking = true;
    }

    public void CastShootingAttack()
    {
        _player.ChangeState<ShootingState>();
    }

    private void Update()
    {
        // 1초 후 다시 처음으로
        if (Time.time > _timeAfterLastBasicAttack + _attackCountRefreshTime)
            _basicAttackCount = 0;
    }

    public void AnimEvent_FinishBaseAttackAnim()
    {
        Debug.Log("Finish");

        IsBasicAttacking = false;

        _player.Animator.SetBool("IsAttack", false);

        _basicAttackHitbox.gameObject.SetActive(false);
    }
}
