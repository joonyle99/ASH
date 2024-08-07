using System.Collections.Generic;
using UnityEngine;

public class Fire_FireBall : Monster_IndependentSkill
{
    private ParticleSystem _ps;
    public ParticleSystem Ps => _ps;

    protected override void Awake()
    {
        base.Awake();

        _ps = GetComponent<ParticleSystem>();
    }

    private void OnParticleTrigger()
    {
        var targetCollider = _ps.trigger.GetCollider(0);

        // 대상이 타겟 레이어인 경우
        var otherLayerValue = 1 << targetCollider.gameObject.layer;
        if ((otherLayerValue & targetLayer.value) > 0)
        {
            // 플레이어와 충돌
            var player = targetCollider.GetComponent<PlayerBehaviour>();
            if (player && !player.IsHurt && !player.IsGodMode && !player.IsDead)
            {
                var particles = new List<ParticleSystem.Particle>();
                var particleCount = _ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);

                var particle = particles[0];

                // Actor가 있으면 Actor를 기준으로 방향 설정
                Vector2 forceVector = (actor == null)
                    ? new Vector2(forceX * Mathf.Sign(player.transform.position.x - particle.position.x),
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

                    particle.remainingLifetime = 0f;
                    particles[0] = particle;
                    _ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);
                }
            }
        }
    }
    private void OnParticleCollision(GameObject other)
    {
        // 대상이 타겟 레이어인 경우
        var otherLayerValue = 1 << other.gameObject.layer;
        if ((otherLayerValue & targetLayer.value) > 0)
        {
            // 플레이어와 충돌
            var player = other.GetComponent<PlayerBehaviour>();
            if (player && !player.IsHurt && !player.IsGodMode && !player.IsDead)
            {
                var collisionEvents = new List<ParticleCollisionEvent>();
                var collisionCount = _ps.GetCollisionEvents(other, collisionEvents);
                var collisionEvent = collisionEvents[0];

                var particles = new ParticleSystem.Particle[_ps.particleCount];
                var particleCount = _ps.GetParticles(particles);
                var particle = particles[0];
                for (int i = 0; i < particleCount; i++)
                {
                    if (particles[i].position.Equals(collisionEvent.intersection))
                    {
                        particle = particles[i];
                        break;
                    }
                }

                // Actor가 있으면 Actor를 기준으로 방향 설정
                Vector2 forceVector = (actor == null)
                    ? new Vector2(forceX * Mathf.Sign(player.transform.position.x - particle.position.x),
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

                    particle.remainingLifetime = 0f;
                }
            }
        }
    }
}