using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerDashController : MonoBehaviour
{
    #region Dash

    // 5. Dash
    [Header("Dash")]
    public float _dashSpeed = 25f;
    public float _dashLength = 0.2f;
    public bool _dashing;

    public float _timeStartedDash;
    public Vector2 _dashDir;

    #endregion

    PlayerBehaviour _player;

    void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
    }

    void Update()
    {
        // Dash Start
        if (Input.GetKeyDown(KeyCode.LeftShift) && !_dashing)
        {
            ExcuteDash();
        }
    }

    public void ExcuteDash()
    {
        _dashing = true;
        _player.Rigidbody.gravityScale = 0; // 중력 0으로 설정
        _dashDir = new Vector2(_player.RawInputs.Movement.x, _player.RawInputs.Movement.y).normalized; // 대쉬 방향 설정

        if (_dashDir == Vector2.zero) // 키보드에 입력이 없이 대쉬를 하는 경우
            _dashDir = (_player.recentDir < 0) ? Vector2.left : Vector2.right;

        _player.Rigidbody.velocity = _dashDir * _dashSpeed; // 대쉬 실행
        _timeStartedDash = Time.time; // Dash를 시작한 시간을 기록
        _player.ChangeState<DashState>();
    }
}
