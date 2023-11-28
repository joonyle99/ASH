using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingSpike : MonoBehaviour, ITriggerListener, IAttackListener
{
    Rigidbody2D _rigidbody;
    [SerializeField] float _raycastWidth = 0.5f;
    [SerializeField] float _fallDelay = 0.3f;
    [SerializeField] float _fallSpeedBonus = 0f;
    [SerializeField] LayerMask _collisionLayers;
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    public void OnHitted(bool isBasicAttack)
    {
        if (isBasicAttack)
            Destroy(gameObject);
    }
    /*
    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter)
    {
        if (activator.MonsterType == ActivatorType.Player)
        {
            Invoke("Fall", _fallDelay);
            Destroy(reporter.gameObject);
        }
    }*/

    void FixedUpdate()
    {
        {
            var hit = Physics2D.Raycast(transform.position + new Vector3(_raycastWidth / 2, 0, 0), Vector2.down, 100, _collisionLayers);
            if (hit && hit.transform.GetComponent<PlayerBehaviour>())
            {
                Invoke("Fall", _fallDelay);
                return;
            }
        }
        {
            var hit = Physics2D.Raycast(transform.position - new Vector3(_raycastWidth / 2, 0, 0), Vector2.down, 100, _collisionLayers);
            if (hit && hit.transform.GetComponent<PlayerBehaviour>())
            {
                Invoke("Fall", _fallDelay);
                return;
            }
        }

    }
    void Fall()
    {
        _rigidbody.AddForce(new Vector2(0, _fallSpeedBonus), ForceMode2D.Impulse);
        _rigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
    }

}
