using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingSpike : MonoBehaviour, ITriggerListener, IAttackListener
{
    Rigidbody2D _rigidbody;

    [SerializeField] float _fallDelay = 0.3f;
    [SerializeField] float _fallSpeedBonus = 0f;
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    public void OnHitted(bool isBasicAttack)
    {
        if (isBasicAttack)
            Destroy(gameObject);
    }

    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter)
    {
        if (activator.Type == ActivatorType.Player)
        {
            Invoke("Fall", _fallDelay);
            Destroy(reporter.gameObject);
        }
    }
    void Fall()
    {
        _rigidbody.AddForce(new Vector2(0, _fallSpeedBonus), ForceMode2D.Impulse);
        _rigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
    }
}
