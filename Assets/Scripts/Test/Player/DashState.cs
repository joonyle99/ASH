using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;


public class DashState : PlayerState
{
    PlayerDashController _dashController;
    PlayerBehaviour _player;
    protected override void OnEnter()
    {
        //Debug.Log("Dash Enter");
        _dashController = GetComponent<PlayerDashController>();
        _player = GetComponent<PlayerBehaviour>();
    }

    protected override void OnUpdate()
    {
        //Debug.Log("Update Dash");

        // Already Dash
        if (_dashController._dashing)
        {
            // End Dash
            if (Time.time >= _dashController._timeStartedDash + _dashController._dashLength)
            {
                _dashController._dashing = false;
                _player.Rigidbody.gravityScale = 5;
                _player.Rigidbody.velocity = new Vector2(_player.Rigidbody.velocity.x, (_player.Rigidbody.velocity.y > 3) ? 3 : _player.Rigidbody.velocity.y);
                _player.ChangeState<InAirState>();
            }
        }
    }
    protected override void OnExit()
    {
        //Debug.Log("Dash Exit");
    }

}