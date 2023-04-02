using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;


public class DashState : PlayerState
{
    #region Dash

    [Header("Dash")]
    [SerializeField] private float _dashSpeed = 25f;
    [SerializeField] private float _dashLength = 0.2f;
    [SerializeField] private bool _dashing = false;

    [SerializeField] private float _timeStartedDash;
    [SerializeField] private Vector2 _dashDir;

    public bool EnableDash;

    #endregion
    
    protected override void OnEnter()
    {
        //Debug.Log("Dash Enter");
        ExcuteDash();
    }

    protected override void OnUpdate()
    {
        //Debug.Log("Update Dash");

        // Already Dash
        if (_dashing)
        {
            // End Dash
            if (Time.time >= _timeStartedDash + _dashLength)
            {
                _dashing = false;
                Player.Rigidbody.gravityScale = 5;
                Player.Rigidbody.velocity = new Vector2(Player.Rigidbody.velocity.x, (Player.Rigidbody.velocity.y > 3) ? 3 : Player.Rigidbody.velocity.y);
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
        Player.Rigidbody.gravityScale = 0; // 중력 0으로 설정
        _dashDir = new Vector2(Player.RawInputs.Movement.x, Player.RawInputs.Movement.y).normalized; // 대쉬 방향 설정

        if (_dashDir == Vector2.zero) // 키보드에 입력이 없이 대쉬를 하는 경우
            _dashDir = (Player.recentDir < 0) ? Vector2.left : Vector2.right;

        Player.Rigidbody.velocity = _dashDir * _dashSpeed; // 대쉬 실행
        _timeStartedDash = Time.time; // Dash를 시작한 시간을 기록
    }
}