using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesolateDiveState : PlayerState
{
    // [SerializeField] Transform _divePoint;
    [SerializeField] LayerMask _enemyLayers;
    [SerializeField] Collider2D[] _targetEnemys;
    [SerializeField] ParticleSystem _particleSystem;

    [SerializeField] float _diveSpeed = 15.0f;
    [SerializeField] float _fastDiveSpeed = 10.0f;
    [SerializeField] float _explosionSizeX = 5.0f;
    [SerializeField] float _explosionSizeY = 1.0f;
    [SerializeField] int _explosionDamage = 10;
    [SerializeField] Vector2 _knockBackVec = new Vector2(0, 10);
    //[SerializeField] float _minHeight = 5.0f;

    bool _isDiving = false;
    bool _isInvincible = false;

    protected override void OnEnter()
    {
        //Debug.Log("Enter Desolate Dive");

        // init dive speed
        Player.Rigidbody.velocity = new Vector2(0, -_diveSpeed);
    }

    protected override void OnUpdate()
    {
        // dive more fast
        Player.Rigidbody.velocity -= Vector2.down * _fastDiveSpeed * Physics2D.gravity.y * Time.deltaTime;

        // end dive => damage / knockback
        if(Player.IsGrounded)
        {
            // create particle system
            Instantiate(_particleSystem, transform.position + new Vector3(0f, 0.5f), Quaternion.identity);

            _targetEnemys = Physics2D.OverlapBoxAll(transform.position, new Vector2(_explosionSizeX, _explosionSizeY), 0, _enemyLayers);

            foreach (Collider2D enemy in _targetEnemys)
            {
                //Debug.Log(enemy.gameObject.name);
                enemy.GetComponent<OncologySlime>().OnDamage(_explosionDamage);
                enemy.GetComponent<OncologySlime>().KnockBack(_knockBackVec);
            }

            ChangeState<IdleState>();
        }
    }
    protected override void OnExit()
    {
        //Debug.Log("Exit Desolate Dive");
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.DrawWireCube(transform.position, new Vector2(_explosionSizeX, _explosionSizeY));
    }
}
