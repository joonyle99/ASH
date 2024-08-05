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
    /// ParticleSystem (���� ���� ��ƼŬ�� ���Ե�) �� �浹�� �� �� ���� ȣ��ȴ�
    /// GetCollisionEvents()�� ���� ������ ��ƼŬ�� �浹 �̺�Ʈ�� ������ �� �ִ�.
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

                // ����� Ÿ�� ���̾��� ���
                if ((layerValue & targetLayer.value) > 0)
                {
                    // �÷��̾ Ÿ�� ������ ������ ���
                    if (!player.IsHurt && !player.IsGodMode && !player.IsDead)
                    {
                        // Actor�� ������ Actor�� �������� ���� ����
                        Vector2 forceVector = (actor == null)
                            ? new Vector2(forceX * Mathf.Sign(player.transform.position.x - this.transform.position.x),
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
                        }
                    }
                }
            }
        }
    }
}