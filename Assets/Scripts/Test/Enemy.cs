using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int _maxHp = 100;
    [SerializeField] int _curHp;
    [SerializeField] bool _dead = false;
    [SerializeField] bool _inAir = false;

    // Start is called before the first frame update
    void Start()
    {
        _curHp = _maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDamage(int _damage)
    {
        //Debug.Log(this.gameObject.name + _damage + "�� �������� �޽��ϴ�");
        _curHp -= _damage;

        if (_curHp <= 0)
        {
            _curHp = 0;
            Die();
        }
    }

    public void KnockBack(Vector2 _kVector)
    {
        //Debug.Log(this.gameObject.name + " �˹� �߻�");
        this.GetComponent<Rigidbody2D>().velocity = _kVector;
        _inAir = true;
    }

    public void Die()
    {
        _dead = true;
        //GetComponent<Collider2D>().enabled = false;
        //this.gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _inAir = false;
        }
    }
}
