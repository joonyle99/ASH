using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class WallSlideState : PlayerState
{
    [SerializeField] private float _wallSlideSpeed = -3f;

    protected override void OnEnter()
    {
        Debug.Log("Enter WallSlide");
    }
    protected override void OnUpdate()
    {
        // �׸��� ������ ���� �������� ��� �߰�
        Player.Rigidbody.velocity = new Vector2(0, _wallSlideSpeed);

        // ���� �������� IdleState��
        if (Player.IsGrounded)
            ChangeState<IdleState>();

        // Grab Input �� WallGrabState��
        //if (grabInput && yInput == 0)
        //    ChangeState<WallGrabState>();
    }

    protected override void OnExit()
    {
        Debug.Log("Exit WallSlide");
    }
}
