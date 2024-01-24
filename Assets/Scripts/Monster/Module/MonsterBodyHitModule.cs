using UnityEngine;

/// <summary>
/// 몬스터의 바디 타격을 담당하는 모듈
/// </summary>
public class MonsterBodyHitModule : MonoBehaviour
{
    [Header("Monster BodyHit Module")]
    [Space]

    [SerializeField] private LayerMask _bodyHitTargetLayer;
    [SerializeField] private GameObject _toHitEffectPrefab;

    [Space]

    [SerializeField] private int _bodyHitDamage = 5;
    [SerializeField] private float _bodyHitForceX = 7f;
    [SerializeField] private float _bodyHitForceY = 9f;

    [Space]

    [SerializeField] private bool _isAttackable = true;
    public bool IsAttackable
    {
        get { return _isAttackable; }
        set { _isAttackable = value; }
    }

    // 대상이 콜라이더에 계속 들어와있을 경우를 고려해 TriggerStay 사용
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(IsAttackable)
        {
            // 타겟 레이어와 충돌
            if ((1 << collision.gameObject.layer & _bodyHitTargetLayer.value) > 0)
            {
                // 타겟이 플레이어인 경우
                PlayerBehaviour player = collision.GetComponent<PlayerBehaviour>();
                if (player)
                {
                    // 플레이어가 타격 가능한 상태인 경우
                    if (!player.IsHurt && !player.IsGodMode && !player.IsDead)
                    {
                        // 플레이어를 타격
                        Vector2 forceVector = new Vector2(_bodyHitForceX * Mathf.Sign(player.transform.position.x - this.transform.position.x), _bodyHitForceY);
                        IAttackListener.AttackResult attackResult = player.OnHit(new AttackInfo(_bodyHitDamage, forceVector, AttackType.Monster_BodyAttack));

                        // 타격 성공 시 히트 이펙트 생성
                        if (attackResult == IAttackListener.AttackResult.Success)
                        {
                            Vector2 playerPos = player.transform.position;
                            Instantiate(_toHitEffectPrefab, playerPos + Random.insideUnitCircle * 0.3f, Quaternion.identity);
                        }
                    }
                }
            }
        }
    }
}