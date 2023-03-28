using StateMahineDemo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    JumpState jump;
    Rigidbody2D _rb;
    Animator _anim;

    // Start is called before the first frame update
    void Start()
    {
        jump = GetComponent<JumpState>();
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Fall faster and allow small jumps. _jumpVelocityFalloff is the point at which we start adding extra gravity. Using 0 causes floating
        // Light Jump & Full Jump
        if ((_rb.velocity.y < jump._jumpVelocityFalloff) || ((_rb.velocity.y > 0) && !Input.GetKey(KeyCode.Space)))
        {
            _rb.velocity += jump._fallMultiplier * Physics2D.gravity.y * Vector2.up * Time.deltaTime;
        }

        _anim.SetFloat("AirSpeedY", _rb.velocity.y);

    }
}
