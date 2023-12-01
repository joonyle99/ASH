using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseForce : MonoBehaviour
{
    [SerializeField] [Range(0f,1f)]float _loseAmount;

    Rigidbody2D _rigidbody;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _rigidbody.velocity *= (1 - _loseAmount);
    }
}
