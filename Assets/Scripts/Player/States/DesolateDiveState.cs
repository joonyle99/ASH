using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
    [SerializeField] int _explosionDamage = 40;
    [SerializeField] float _knockBackPower = 10f;
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

            // damage range
            _targetEnemys = Physics2D.OverlapBoxAll(transform.position, new Vector2(_explosionSizeX, _explosionSizeY), 0, _enemyLayers);

            // target을 전부 순회
            foreach (Collider2D enemy in _targetEnemys)
            {
                float dir = Mathf.Sign(enemy.transform.position.x - transform.position.x);

                Vector2 vec = new Vector2(_knockBackPower * dir, _knockBackPower / 2f);

                // 만약 슬라임이면
                if (enemy.GetComponent<OncologySlime>() != null)
                {

                }

                //Debug.Log(enemy.gameObject.name);
                enemy.GetComponent<OncologySlime>().OnDamage(_explosionDamage);
                enemy.GetComponent<OncologySlime>().KnockBack(vec);
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
