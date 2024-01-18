using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class FallingSpike : MonoBehaviour, ITriggerListener, IAttackListener
{
    Rigidbody2D _rigidbody;
    [SerializeField] float _raycastWidth = 0.5f;
    [SerializeField] float _fallDelay = 0.3f;
    [SerializeField] float _fallSpeedBonus = 0f;
    [SerializeField] LayerMask _collisionLayers;
    [SerializeField] SoundList _soundList;

    bool _isFallInvoked = false;
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    public IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        if (attackInfo.Type == AttackType.BasicAttack)
        {
            Destroy(gameObject);
            return IAttackListener.AttackResult.Success;
        }

        return IAttackListener.AttackResult.Fail;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        _soundList.PlaySFX("SE_FallingSpike_Land");
    }

    void FixedUpdate()
    {
        if (_isFallInvoked)
            return;
        {
            var hit = Physics2D.Raycast(transform.position + new Vector3(_raycastWidth / 2, 0, 0), Vector2.down, 100, _collisionLayers);
            if (hit && hit.transform.GetComponent<PlayerBehaviour>())
            {
                StartCoroutine(InvokeFallCoroutine());
                return;
            }
        }
        {
            var hit = Physics2D.Raycast(transform.position - new Vector3(_raycastWidth / 2, 0, 0), Vector2.down, 100, _collisionLayers);
            if (hit && hit.transform.GetComponent<PlayerBehaviour>())
            {
                StartCoroutine(InvokeFallCoroutine());
                return;
            }
        }

    }
    IEnumerator InvokeFallCoroutine()
    {
        _isFallInvoked = true;
        _soundList.PlaySFX("SE_FallingSpike_Broke");
        yield return new WaitForSeconds(_fallDelay);
        Fall();
    }
    void Fall()
    {
        _rigidbody.AddForce(new Vector2(0, _fallSpeedBonus), ForceMode2D.Impulse);
        _rigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
    }

}
