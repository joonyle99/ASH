using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;


public class DashState : PlayerState
{
    #region Dash

    [SerializeField] private float _dashSpeed = 20f;
    [SerializeField] private float _dashLength = 0.2f;
    [SerializeField] private float _coolTime = 0.5f;

    private bool _dashing = false;
    private float _timeStartedDash;
    private float _timeEndeddDash;
    private Vector2 _dashDir;

    public bool EnableDash = true;

    public bool Dashing { get { return _dashing; } }
    public float CoolTime { get { return _coolTime; } }
    public float TimeEndedDash { get { return _timeEndeddDash; } }

    #endregion
    
    protected override void OnEnter()
    {
        Debug.Log("Dash Enter");
        ExcuteDash();
    }

    protected override void OnUpdate()
    {
        //Debug.Log("Update Dash");

        // Already Dash
        if (_dashing)
        {
            // End Dash (�뽬�� ������ ����)
            if (Time.time >= _timeStartedDash + _dashLength)
            {
                _dashing = false;
                _timeEndeddDash = Time.time; // �뽬�� ������ ������ �ð�
                Player.Rigidbody.gravityScale = 5;
                Player.Rigidbody.velocity = new Vector2(Player.Rigidbody.velocity.x, Player.Rigidbody.velocity.y);
                Player.ChangeState<InAirState>();
            }
        }
    }

    protected override void OnExit()
    {
        //Debug.Log("Dash Exit");
    }

    public void ExcuteDash()
    {
        _dashing = true;
        EnableDash = false;
        Player.Rigidbody.gravityScale = 0; // �߷� 0���� ����
        _dashDir = new Vector2(Player.RawInputs.Movement.x, 0).normalized; // �뽬 ���� ����

        //if (_dashDir == Vector2.zero) // Ű���忡 �Է��� ���� �뽬�� �ϴ� ���
        //    _dashDir = (Player.RecentDir.x < 0) ? Vector2.left : Vector2.right;

        Player.Rigidbody.velocity = _dashDir * _dashSpeed; // �뽬 ����
        _timeStartedDash = Time.time; // Dash�� ������ �ð��� ���
    }
}