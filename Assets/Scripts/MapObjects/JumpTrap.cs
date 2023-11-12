using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTrap : MonoBehaviour
{
    [SerializeField] Collider2D [] _leafColliders;

    [SerializeField] float _jumpPower = 10;
    [SerializeField] float _closeDelay = 1;
    [SerializeField] float _openDelay = 1;

    Animator _animator;
    bool _isOpen = true;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isOpen && collision.transform.GetComponent<PlayerBehaviour>() != null)
        {
            collision.rigidbody.AddForce(new Vector2(0, _jumpPower), ForceMode2D.Impulse);
            Invoke("CloseTrap", _closeDelay);
        }
    }

    void CloseTrap()
    {
        _isOpen = false;
        foreach (var collider in _leafColliders)
            collider.enabled = true;
        _animator.SetTrigger("Close");
    }

    public void AnimEvent_InvokeOpen()
    {
        Invoke("OpenTrap", _openDelay);
    }

    void OpenTrap()
    {
        _isOpen = true;
        foreach (var collider in _leafColliders)
            collider.enabled = false;
        _animator.SetTrigger("Open");
    }
}
