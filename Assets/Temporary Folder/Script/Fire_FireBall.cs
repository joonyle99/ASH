using System.Collections.Generic;
using UnityEngine;

public class Fire_FireBall : Monster_IndependentSkill
{
    private ParticleSystem _ps;

    protected override void Awake()
    {
        base.Awake();

        _ps = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// ParticleSystem (여러 개의 파티클이 포함된) 이 충돌할 때 한 번만 호출된다
    /// GetCollisionEvents()를 통해 각각의 파티클의 충돌 이벤트를 가져올 수 있다.
    /// </summary>
    private void OnParticleCollision()
    {
        var eventList = new List<ParticleCollisionEvent>();

        var player = SceneContext.Current.Player;
        if (player == null) return;

        var eventCount = _ps.GetCollisionEvents(player.gameObject, eventList);
        if (eventCount > 0)
        {
            foreach (var collisionEvent in eventList)
            {
                var targetCollision = collisionEvent.colliderComponent;
                var layerValue = 1 << targetCollision.gameObject.layer;

                // 대상이 타겟 레이어인 경우
                if ((layerValue & targetLayer.value) > 0)
                {
                    // 플레이어가 타격 가능한 상태인 경우
                    if (!player.IsHurt && !player.IsGodMode && !player.IsDead)
                    {
                        // Actor가 있으면 Actor를 기준으로 방향 설정
                        Vector2 forceVector = (actor == null)
                            ? new Vector2(forceX * Mathf.Sign(player.transform.position.x - this.transform.position.x),
                                forceY)
                            : new Vector2(forceX * Mathf.Sign(player.transform.position.x - actor.transform.position.x),
                                forceY);

                        // 플레이어를 타격
                        var attackInfo = new AttackInfo(damage, forceVector, AttackType.Monster_SkillAttack);
                        IAttackListener.AttackResult attackResult = player.OnHit(attackInfo);

                        // 타격 성공
                        if (attackResult == IAttackListener.AttackResult.Success)
                        {
                            // 피격 이펙트 생성
                            Vector2 playerPos = player.transform.position;
                            var effect = Instantiate(hitEffect, playerPos + Random.insideUnitCircle * 0.3f, Quaternion.identity);

                            // 스킬이 플레이어를 타격한 경우 발생하는 이벤트
                            monsterSkillEvent?.Invoke();
                        }
                    }
                }
            }
        }
    }
}