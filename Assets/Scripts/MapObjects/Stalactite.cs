using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Stalactite : MonoBehaviour
{
    [Space(10), Header("Setting")]
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected LayerMask destroyLayer;

    [SerializeField] private float _lifeTime;                  // 투사체가 살아있는 시간
    [SerializeField] private float _effectDelay;
    [SerializeField] private Vector3 _knockbackVector;
    [SerializeField] private float _damage;

    [SerializeField] private bool _isReachedAtGround = false;


    [Space(10), Header("Reference")]
    private MaterialController materialController;
    private Coroutine _autoDestroyCoroutine;

    [Space(10), Header("Prefab")]
    [SerializeField] protected GameObject _hitEffect;

    protected void Awake()
    {
        materialController = GetComponent<MaterialController>();
    }
    private void Start()
    {
        // 스킬이 생성되고 일정 시간이 지나면 자동으로 파괴
        _autoDestroyCoroutine = StartCoroutine(AutoDestroy(_lifeTime));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isReachedAtGround) return;

        // 스킬 타겟 레이어와 충돌
        var collisionLayerValue = 1 << collision.gameObject.layer;
        if ((collisionLayerValue & targetLayer.value) > 0)
        {
            // 타겟이 플레이어인 경우
            PlayerBehaviour player = collision.collider.GetComponent<PlayerBehaviour>();
            if (player)
            {
                // 플레이어가 타격 가능한 상태인 경우
                if (!player.IsHurt && !player.IsGodMode && !player.IsDead)
                {
                    // 플레이어를 타격
                    var attackInfo = new AttackInfo(_damage, _knockbackVector, AttackType.Monster_SkillAttack);
                    IAttackListener.AttackResult attackResult = player.OnHit(attackInfo);

                    // 타격 성공
                    if (attackResult == IAttackListener.AttackResult.Success)
                    {
                        // 피격 이펙트 생성
                        Vector2 playerPos = player.transform.position;
                        var effect = Instantiate(_hitEffect, playerPos + Random.insideUnitCircle * 0.3f, Quaternion.identity);

                        // 타겟 레이어를 히트와 동시에 파괴
                        if ((collisionLayerValue & destroyLayer.value) > 0)
                        {
                            Destroy(gameObject);
                        }
                    }
                }
            }
        }

        collisionLayerValue = 1 << collision.gameObject.layer;
        if ((collisionLayerValue & destroyLayer.value) > 0)
        {
            //땅에 닿았을 경우
            _isReachedAtGround = true;

            if (materialController == null || materialController.DisintegrateEffect == null)
            {
                Destroy(gameObject);
            }
            else
            {
                StopAutoDestroy();
                StartCoroutine(DisintegrateCoroutine(materialController.DisintegrateEffect));
            }
        }
    }

    public void SetLifeTime(float time)
    {
        _lifeTime = time;
    }
    public void SetEffectDelay(float delay)
    {
        _effectDelay = delay;
    }

    private IEnumerator AutoDestroy(float time)
    {
        if (time < 0.1f) yield break;

        yield return new WaitForSeconds(time);

        Destroy(gameObject);
    }
    private void StopAutoDestroy()
    {
        if (_autoDestroyCoroutine != null)
        {
            StopCoroutine(_autoDestroyCoroutine);
            _autoDestroyCoroutine = null;
        }
    }

    private IEnumerator DisintegrateCoroutine(DisintegrateEffect effect)
    {
        yield return DestroyEffectCoroutine(effect);

        Destroy(gameObject);
    }
    private IEnumerator DestroyEffectCoroutine(DisintegrateEffect effect)
    {
        effect.Play(_effectDelay);
        yield return new WaitUntil(() => effect.IsEffectDone);
    }
}
