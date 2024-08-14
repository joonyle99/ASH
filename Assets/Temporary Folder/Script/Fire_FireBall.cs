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

        // ����� Ÿ�� ���̾��� ���
        var otherLayerValue = 1 << targetCollider.gameObject.layer;
        if ((otherLayerValue & targetLayer.value) > 0)
        {
            // �÷��̾�� �浹
            var player = targetCollider.GetComponent<PlayerBehaviour>();
            if (player && !player.IsHurt && !player.IsGodMode && !player.IsDead)
            {
                var particles = new List<ParticleSystem.Particle>();
                var particleCount = _ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);

                var particle = particles[0];

                // Actor�� ������ Actor�� �������� ���� ����
                Vector2 forceVector = (actor == null)
                    ? new Vector2(forceX * Mathf.Sign(player.transform.position.x - particle.position.x),
                        forceY)
                    : new Vector2(forceX * Mathf.Sign(player.transform.position.x - actor.transform.position.x),
                        forceY);

                // �÷��̾ Ÿ��
                var attackInfo = new AttackInfo(damage, forceVector, AttackType.Monster_SkillAttack);
                IAttackListener.AttackResult attackResult = player.OnHit(attackInfo);

                // Ÿ�� ����
                if (attackResult == IAttackListener.AttackResult.Success)
                {
                    // �ǰ� ����Ʈ ����
                    Vector2 playerPos = player.transform.position;
                    var effect = Instantiate(hitEffect, playerPos + Random.insideUnitCircle * 0.3f, Quaternion.identity);

                    // ��ų�� �÷��̾ Ÿ���� ��� �߻��ϴ� �̺�Ʈ
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
        // ����� Ÿ�� ���̾��� ���
        var otherLayerValue = 1 << other.gameObject.layer;
        if ((otherLayerValue & targetLayer.value) > 0)
        {
            // �÷��̾�� �浹
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

                // Actor�� ������ Actor�� �������� ���� ����
                Vector2 forceVector = (actor == null)
                    ? new Vector2(forceX * Mathf.Sign(player.transform.position.x - particle.position.x),
                        forceY)
                    : new Vector2(forceX * Mathf.Sign(player.transform.position.x - actor.transform.position.x),
                        forceY);

                // �÷��̾ Ÿ��
                var attackInfo = new AttackInfo(damage, forceVector, AttackType.Monster_SkillAttack);
                IAttackListener.AttackResult attackResult = player.OnHit(attackInfo);

                // Ÿ�� ����
                if (attackResult == IAttackListener.AttackResult.Success)
                {
                    // �ǰ� ����Ʈ ����
                    Vector2 playerPos = player.transform.position;
                    var effect = Instantiate(hitEffect, playerPos + Random.insideUnitCircle * 0.3f, Quaternion.identity);

                    // ��ų�� �÷��̾ Ÿ���� ��� �߻��ϴ� �̺�Ʈ
                    monsterSkillEvent?.Invoke();

                    particle.remainingLifetime = 0f;
                }
            }
        }
    }
}