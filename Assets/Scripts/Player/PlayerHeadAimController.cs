using System;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

public class PlayerHeadAimController : MonoBehaviour
{
    [Header("HeadAim")]
    [Space]

    public float headAimValue = 0f;
    public float headAimSpeed = 2f;

    [Space]

    public float cameraMoveSpeed = 1f;
    public float cameraReturnMoveSpeed = 2f;
    public float cameraOffsetLimit = 0.3f;

    [Space]

    private float _headAimModifier = 0f;
    private float _cameraMoveDir = 0f;

    private float _defaultHeadAimValue = 0f;
    private float _defaultCameraOffsetY = 0f;

    private PlayerBehaviour _player;
    private PlayerLightSkillController _playerLightSkillController;

    public bool ShouldControlHeadAim => _player.CurrentStateIs<IdleState>() &&
                                        _playerLightSkillController.IsLightButtonPressable &&
                                        !_playerLightSkillController.IsLightWorking && _player.IsMoveYKey;

    void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
        _playerLightSkillController = _player.GetComponent<PlayerLightSkillController>();
    }
    void Start()
    {
        _defaultCameraOffsetY = SceneContext.Current.Camera.OffsetY;
        _defaultHeadAimValue = _player.Animator.GetFloat("HeadAimValue");

        headAimValue = _defaultHeadAimValue;
    }
    void Update()
    {
        if (ShouldControlHeadAim)
        {
            if (_player.IsMoveUpKey)
            {
                _headAimModifier = 1f;
                _cameraMoveDir = 1f;
            }
            else if (_player.IsMoveDownKey)
            {
                _headAimModifier = -1f;
                _cameraMoveDir = -1f;
            }

            HeadAimValueControl();
        }
        else
        {
            HeadAimValueReturn();
        }
    }
    void LateUpdate()
    {
        if (ShouldControlHeadAim)
        {
            CameraOffsetControl();
        }
        else
        {
            CameraOffsetReturn();
        }
    }

    private void HeadAimValueControl()
    {
        headAimValue += Time.deltaTime * headAimSpeed * _headAimModifier;
        headAimValue = Mathf.Clamp01(headAimValue);

        _player.Animator.SetFloat("HeadAimValue", headAimValue);
    }
    private void HeadAimValueReturn()
    {
        _headAimModifier = headAimValue > _defaultHeadAimValue ? -1f : 1f;

        if (Mathf.Abs(headAimValue - _defaultHeadAimValue) > 0.001f)
        {
            headAimValue += Time.deltaTime * headAimSpeed * _headAimModifier;
            _player.Animator.SetFloat("HeadAimValue", headAimValue);
        }
    }

    private void CameraOffsetControl()
    {
        var cameraPosY = SceneContext.Current.Camera.OffsetY;

        cameraPosY += Time.deltaTime * cameraMoveSpeed * _cameraMoveDir;
        cameraPosY = Mathf.Clamp(cameraPosY, _defaultCameraOffsetY - cameraOffsetLimit,
            _defaultCameraOffsetY + cameraOffsetLimit);
        cameraPosY = Mathf.Clamp(cameraPosY, -1f, 1f);

        SceneContext.Current.Camera.OffsetY = cameraPosY;
    }
    private void CameraOffsetReturn()
    {
        var cameraPosY = SceneContext.Current.Camera.OffsetY;
        _cameraMoveDir = cameraPosY > _defaultCameraOffsetY ? -1f : 1f;

        if (Mathf.Abs(cameraPosY - _defaultCameraOffsetY) > 0.001f)
        {
            cameraPosY += Time.deltaTime * cameraReturnMoveSpeed * _cameraMoveDir;
            SceneContext.Current.Camera.OffsetY = cameraPosY;
        }
    }
}
