using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DashState : PlayerState
{
    #region Dash

    // 5. Dash
    [Header("Dash")]
    public float _dashSpeed = 13f;
    public float _dashLength = 0.2f;
    public bool _hasDashed;
    public bool _dashing;
    public bool _useGravity;

    public float _timeStartedDash;
    public Vector2 _dashDir;

    #endregion

    protected override void OnEnter()
    {

    }
    protected override void OnExit()
    {

    }

    protected override void OnUpdate()
    {

    }
}