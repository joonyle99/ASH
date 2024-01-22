using UnityEngine;

public class Mushroom_DevourAttack : Monster_SkillAttack
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // base.OnTriggerEnter2D(collision);

        // 스킬 타겟 레이어와 충돌
        if ((1 << collision.gameObject.layer & _skillTargetLayer.value) > 0)
        {
            // 타겟이 플레이어인 경우
            PlayerBehaviour player = collision.GetComponent<PlayerBehaviour>();
            if (player)
            {
                // 플레이어가 타격 가능한 상태인 경우
                if (!player.IsHurt && !player.IsGodMode && !player.IsDead)
                {
                    // 플레이어를 타격
                    Transform mushroomTrans = this.transform.root;
                    Vector2 forceVector = new Vector2(_skillForceX * Mathf.Sign(player.transform.position.x - mushroomTrans.position.x), _skillForceY);
                    IAttackListener.AttackResult attackResult = player.OnHit(new AttackInfo(_skillDamage, forceVector, AttackType.Monster_SkillAttack));

                    // 타격 성공 시 히트 이펙트 생성
                    if (attackResult == IAttackListener.AttackResult.Success)
                    {
                        Vector2 playerPos = player.transform.position;
                        Instantiate(_hitEffectPrefab, playerPos + Random.insideUnitCircle * 0.3f, Quaternion.identity);
                    }
                }
            }
        }

        // 스킬 파괴 레이어와 충돌
        if ((1 << collision.gameObject.layer & _skillDestroyLayer.value) > 0)
            Destroy(this.gameObject);
    }
}
