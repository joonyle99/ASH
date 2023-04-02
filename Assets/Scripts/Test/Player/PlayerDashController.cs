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
        _player.Rigidbody.gravityScale = 0; // �߷� 0���� ����
        _dashDir = new Vector2(_player.RawInputs.Movement.x, _player.RawInputs.Movement.y).normalized; // �뽬 ���� ����

        if (_dashDir == Vector2.zero) // Ű���忡 �Է��� ���� �뽬�� �ϴ� ���
            _dashDir = (_player.recentDir < 0) ? Vector2.left : Vector2.right;

        _player.Rigidbody.velocity = _dashDir * _dashSpeed; // �뽬 ����
        _timeStartedDash = Time.time; // Dash�� ������ �ð��� ���
        _player.ChangeState<DashState>();
    }
}
