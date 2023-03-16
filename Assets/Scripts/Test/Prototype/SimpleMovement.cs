using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    [SerializeField] public float MoveSpeed = 4;
    void Update()
    {
        Vector3 moveVector = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            moveVector.y += 1;
        if (Input.GetKey(KeyCode.S))
            moveVector.y -= 1;
        if (Input.GetKey(KeyCode.A))
            moveVector.x -= 1;
        if (Input.GetKey(KeyCode.D))
            moveVector.x += 1;

        transform.position += moveVector * MoveSpeed * Time.deltaTime;
    }
}
