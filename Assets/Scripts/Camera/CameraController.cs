using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Com.LuisPedroFonseca.ProCamera2D;
using JetBrains.Annotations;
using Unity.VisualScripting.Antlr3.Runtime;


public class CameraController : MonoBehaviour, ISceneContextBuildListener
{
    ProCamera2D _proCamera;
    ProCamera2DShake _shakeComponent;

    public float OffsetX
    {
        get { return _proCamera.OffsetX; }
        set { _proCamera.OffsetX = value;}
    }
    public float OffsetY
    {
        get { return _proCamera.OffsetY; }
        set { _proCamera.OffsetY = value; }
    }
    struct InitialSettings
    {
        public Vector2 Offset;
        public Vector2 FollowSmoothness;
    }
    InitialSettings _initialSettings;

    void Awake()
    {
        _proCamera = GetComponent<ProCamera2D>();
        _shakeComponent = _proCamera.GetComponent<ProCamera2DShake>();
        _initialSettings.Offset = new Vector2(OffsetX, OffsetY);
        _initialSettings.FollowSmoothness = new Vector2(_proCamera.HorizontalFollowSmoothness, _proCamera.VerticalFollowSmoothness);
    }
    public void OnSceneContextBuilt()
    {
        CameraControlToken.ClearQueue();
        _proCamera.enabled = true;
        _proCamera.AddCameraTarget(SceneContext.Current.Player.transform);
    }
    public void StartFollow(Transform target, bool removeExisting = true)
    {
        if (removeExisting)
            _proCamera.RemoveAllCameraTargets();
        _proCamera.AddCameraTarget(target);
    }
    public void DisableCameraFollow()
    {
        _proCamera.HorizontalFollowSmoothness = 100f;
        _proCamera.VerticalFollowSmoothness = 100f;
    }
    public void SnapFollow()
    {
        StartCoroutine(SnapFollowCoroutine());
    }
    IEnumerator SnapFollowCoroutine()
    {
        float originalSmoothnessX = _proCamera.HorizontalFollowSmoothness;
        float originalSmoothnessY = _proCamera.VerticalFollowSmoothness;
        _proCamera.HorizontalFollowSmoothness = 0;
        _proCamera.VerticalFollowSmoothness = 0;
        yield return null;
        _proCamera.HorizontalFollowSmoothness = originalSmoothnessX;
        _proCamera.VerticalFollowSmoothness = originalSmoothnessY;

    }
    public void StartShake(ShakePreset preset)
    {
        _shakeComponent.Shake(preset);
    }
    public void StartConstantShake(ConstantShakePreset preset)
    {
        _shakeComponent.StopConstantShaking(0.3f);
        _shakeComponent.ConstantShake(preset);
    }
    public void StopConstantShake(float smooth = 0.1f)
    {
        _shakeComponent.StopConstantShaking(smooth);
    }
    public void ResetCameraSettings()
    {
        StartFollow(SceneContext.Current.Player.transform);

        OffsetX = _initialSettings.Offset.x;
        OffsetY = _initialSettings.Offset.y;
        _proCamera.HorizontalFollowSmoothness = _initialSettings.FollowSmoothness.x;
        _proCamera.VerticalFollowSmoothness = _initialSettings.FollowSmoothness.y;
    }


}
