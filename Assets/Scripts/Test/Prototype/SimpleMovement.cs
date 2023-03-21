using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    [SerializeField] public float MoveSpeed = 4;
    private void OnEnable()
    {
        InputManager.Instance.JumpPressedEvent += OnJumpPressed;
    }
    void Update()
    {
        var inputState = InputManager.Instance.GetState();
        Vector3 moveVector = inputState.Movement;
        transform.position += moveVector * MoveSpeed * Time.deltaTime;
    }

    void OnJumpPressed()
    {
        Debug.Log("JumpPressed");
    }
}
